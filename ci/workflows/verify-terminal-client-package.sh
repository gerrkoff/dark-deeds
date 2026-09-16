#!/usr/bin/env bash

set -euo pipefail

package_dir="$1"
version="$2"
runtime_identifier="$3"

if [[ "$(uname -s)" == MINGW* || "$(uname -s)" == MSYS* || "$(uname -s)" == CYGWIN* ]]; then
  package_dir="$(cygpath -u "$package_dir")"
fi

if [[ "$runtime_identifier" == win-* ]]; then
  executable_name="dd-terminal.exe"
  archive_name="dd-terminal-$runtime_identifier.zip"
else
  executable_name="dd-terminal"
  archive_name="dd-terminal-$runtime_identifier.tar.gz"
fi

archive_path="$package_dir/$archive_name"
checksum_path="$archive_path.sha256"

if [[ "$runtime_identifier" == win-* ]]; then
  ( cd "$package_dir" && sha256sum -c "$(basename "$checksum_path")" )
else
  ( cd "$package_dir" && shasum -a 256 -c "$(basename "$checksum_path")" )
fi

extract_dir="$(mktemp -d)"
cleanup() { rm -rf "$extract_dir"; }
trap cleanup EXIT

if [[ "$runtime_identifier" == win-* ]]; then
  archive_entries="$(7z l -slt "$archive_path" | sed -n 's/^Path = //p' | tail -n +2)"
  7z x -y "-o$extract_dir" "$archive_path" >/dev/null
else
  archive_entries="$(tar -tzf "$archive_path")"
  tar -xzf "$archive_path" -C "$extract_dir"
fi

if [ "$archive_entries" != "$executable_name" ]; then
  echo "error: archive must contain only $executable_name, found: $archive_entries" >&2
  exit 1
fi

binary="$extract_dir/$executable_name"
actual_version="$("$binary" --version | tr -d '\r')"
if [ "$actual_version" != "$version" ]; then
  echo "error: expected version $version, got $actual_version" >&2
  exit 1
fi

"$binary" --help | tr -d '\r' | grep -q 'keyboard-first terminal client'

if [ "$(uname -s)" = "Darwin" ]; then
  unexpected_libraries="$(
    otool -L "$binary" |
      tail -n +2 |
      awk '{print $1}' |
      grep -Ev '^(/usr/lib/|/System/Library/)' || true
  )"
  if [ -n "$unexpected_libraries" ]; then
    echo "error: macOS binary links non-system libraries: $unexpected_libraries" >&2
    exit 1
  fi
fi

echo "Verified $archive_name"
