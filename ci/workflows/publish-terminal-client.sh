#!/usr/bin/env bash
#
# publish-terminal-client.sh
#
# Builds one self-contained NativeAOT binary of the Dark Deeds terminal client and writes one
# deterministic platform archive plus a matching .sha256 checksum into the given output directory.
#
# Usage: ci/workflows/publish-terminal-client.sh <output-directory> <version> <runtime-identifier>
#
# This script is an internal helper for the terminal release workflow. The workflow owns runner/RID
# selection and installs the required SDK and native toolchain.

set -euo pipefail

readonly OUTPUT_ARGUMENT="$1"
readonly VERSION="$2"
readonly RUNTIME_IDENTIFIER="$3"
readonly BINARY_NAME="dd-terminal"
readonly CONFIGURATION="Release"
# Fixed modification time (2000-01-01 00:00:00) so archive contents are byte-stable across runs.
readonly DETERMINISTIC_TIMESTAMP="200001010000.00"

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/../.." && pwd)"
backend_dir="$repo_root/code/backend"

output_arg="$OUTPUT_ARGUMENT"
if [[ "$(uname -s)" == MINGW* || "$(uname -s)" == MSYS* || "$(uname -s)" == CYGWIN* ]]; then
    output_arg="$(cygpath -u "$output_arg")"
fi
mkdir -p "$output_arg"
output_dir="$(cd "$output_arg" && pwd)"

if [[ "$RUNTIME_IDENTIFIER" == win-* ]]; then
    checksum() { sha256sum "$1"; }
else
    checksum() { shasum -a 256 "$1"; }
fi

publish_root="$(mktemp -d)"
cleanup() { rm -rf "$publish_root"; }
trap cleanup EXIT

echo "Publishing $BINARY_NAME $VERSION for $RUNTIME_IDENTIFIER..."
publish_dir="$publish_root/$RUNTIME_IDENTIFIER"

(
    cd "$backend_dir"
    dotnet publish "DD.TerminalClient/DD.TerminalClient.csproj" \
        --configuration "$CONFIGURATION" \
        --runtime "$RUNTIME_IDENTIFIER" \
        --property:Version="$VERSION" \
        --property:IncludeSourceRevisionInInformationalVersion=false \
        --nologo \
        --output "$publish_dir"
)

if [[ "$RUNTIME_IDENTIFIER" == win-* ]]; then
    executable_name="$BINARY_NAME.exe"
    produced="$publish_dir/DD.TerminalClient.exe"
else
    executable_name="$BINARY_NAME"
    produced="$publish_dir/DD.TerminalClient"
fi

binary="$publish_dir/$executable_name"
mv "$produced" "$binary"
if [[ "$RUNTIME_IDENTIFIER" != win-* ]]; then
    chmod 0755 "$binary"
fi
touch -t "$DETERMINISTIC_TIMESTAMP" "$binary"

if [[ "$RUNTIME_IDENTIFIER" == win-* ]]; then
    archive_name="$BINARY_NAME-$RUNTIME_IDENTIFIER.zip"
    archive_path="$output_dir/$archive_name"
    rm -f "$archive_path"
    ( cd "$publish_dir" && 7z a -tzip -mx=9 "$archive_path" "$executable_name" >/dev/null )
else
    archive_name="$BINARY_NAME-$RUNTIME_IDENTIFIER.tar.gz"
    archive_path="$output_dir/$archive_name"
    if tar --version 2>/dev/null | grep -qi 'gnu tar'; then
        tar_owner_flags=(--format=ustar --numeric-owner --owner=0 --group=0)
    else
        tar_owner_flags=(--format ustar --numeric-owner --uid 0 --gid 0)
    fi
    tar "${tar_owner_flags[@]}" -C "$publish_dir" -cf - "$executable_name" |
        gzip -9 -n -c > "$archive_path"
fi

# Reference only the archive basename so the checksum verifies from the output directory.
( cd "$output_dir" && checksum "$archive_name" > "$archive_name.sha256" )

echo "Created $archive_path"
