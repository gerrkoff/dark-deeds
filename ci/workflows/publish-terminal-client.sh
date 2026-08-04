#!/usr/bin/env bash
#
# publish-terminal-client.sh
#
# Builds untrimmed, self-contained, single-file binaries of the Dark Deeds terminal client for every
# supported runtime identifier and writes one deterministic platform archive plus a matching .sha256
# checksum file per runtime identifier into the given output directory.
#
# Usage: ci/workflows/publish-terminal-client.sh <output-directory>
#
# The script is intended to be runnable locally (macOS or Linux) and from the release workflow.

set -euo pipefail

readonly BINARY_NAME="dd-terminal"
readonly CONFIGURATION="Release"
readonly RUNTIME_IDENTIFIERS="osx-arm64 osx-x64 linux-x64 linux-arm64 win-x64"
# Fixed modification time (2000-01-01 00:00:00) so archive contents are byte-stable across runs.
readonly DETERMINISTIC_TIMESTAMP="200001010000.00"

usage() {
    cat <<'USAGE'
Usage: publish-terminal-client.sh <output-directory>

Builds the Dark Deeds terminal client for osx-arm64, osx-x64, linux-x64, linux-arm64, and win-x64.
Unix binaries are packaged as .tar.gz; the Windows binary is packaged as .zip. A matching .sha256
checksum is written for every archive.
USAGE
}

if [ "${1:-}" = "-h" ] || [ "${1:-}" = "--help" ]; then
    usage
    exit 0
fi

if [ "$#" -ne 1 ]; then
    usage >&2
    exit 2
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/../.." && pwd)"
backend_dir="$repo_root/code/backend"
project="$backend_dir/DD.TerminalClient/DD.TerminalClient.csproj"

if [ ! -f "$project" ]; then
    echo "error: project not found at $project" >&2
    exit 1
fi

mkdir -p "$1"
output_dir="$(cd "$1" && pwd)"

if ! command -v zip >/dev/null 2>&1; then
    echo "error: zip is required to package the Windows binary" >&2
    exit 1
fi

# Pick the ownership-normalization flags for the local tar flavor so archives do not embed the
# building user's uid/gid. GNU tar and BSD (macOS) tar spell these options differently.
if tar --version 2>/dev/null | grep -qi 'gnu tar'; then
    tar_owner_flags=(--format=ustar --numeric-owner --owner=0 --group=0)
else
    tar_owner_flags=(--format ustar --numeric-owner --uid 0 --gid 0)
fi

# Pick an available SHA-256 tool (sha256sum on Linux, shasum on macOS).
if command -v sha256sum >/dev/null 2>&1; then
    checksum() { sha256sum "$1"; }
elif command -v shasum >/dev/null 2>&1; then
    checksum() { shasum -a 256 "$1"; }
else
    echo "error: neither sha256sum nor shasum is available" >&2
    exit 1
fi

publish_root="$(mktemp -d)"
cleanup() { rm -rf "$publish_root"; }
trap cleanup EXIT

for rid in $RUNTIME_IDENTIFIERS; do
    echo "Publishing $BINARY_NAME for $rid..."
    publish_dir="$publish_root/$rid"

    (
        cd "$backend_dir"
        dotnet publish "DD.TerminalClient/DD.TerminalClient.csproj" \
            --configuration "$CONFIGURATION" \
            --runtime "$rid" \
            --nologo \
            --output "$publish_dir"
    )

    if [[ "$rid" == win-* ]]; then
        executable_name="$BINARY_NAME.exe"
        produced="$publish_dir/DD.TerminalClient.exe"
    else
        executable_name="$BINARY_NAME"
        produced="$publish_dir/DD.TerminalClient"
    fi

    # The single-file apphost is emitted under the assembly name; rename it to the distributed
    # executable name so archives ship a consistent, user-facing binary.
    if [ ! -f "$produced" ]; then
        echo "error: expected published binary not found at $produced" >&2
        exit 1
    fi

    binary="$publish_dir/$executable_name"
    mv "$produced" "$binary"
    chmod 0755 "$binary"
    touch -t "$DETERMINISTIC_TIMESTAMP" "$binary"

    if [[ "$rid" == win-* ]]; then
        archive_name="$BINARY_NAME-$rid.zip"
        archive_path="$output_dir/$archive_name"
        ( cd "$publish_dir" && zip -X -9 -q "$archive_path" "$executable_name" )
    else
        archive_name="$BINARY_NAME-$rid.tar.gz"
        archive_path="$output_dir/$archive_name"
        tar "${tar_owner_flags[@]}" -C "$publish_dir" -cf - "$executable_name" | gzip -9 -n -c > "$archive_path"
    fi

    # Reference only the archive basename in the checksum file so it verifies from the output
    # directory with `shasum -a 256 -c <name>.sha256`.
    ( cd "$output_dir" && checksum "$archive_name" > "$archive_name.sha256" )

    echo "Created $archive_path"
done

echo "Done. Archives and checksums are in $output_dir"
