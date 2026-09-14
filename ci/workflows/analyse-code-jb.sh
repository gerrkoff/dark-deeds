#!/usr/bin/env bash

set -euo pipefail

usage() {
  cat <<'USAGE'
Usage: ci/workflows/analyse-code-jb.sh <repository-relative-solution-path>

Runs JetBrains InspectCode for a .NET solution and fails if it finds any Warning or Error issues.
USAGE
}

if [ "$#" -ne 1 ]; then
  usage >&2
  exit 2
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/../.." && pwd)"
solution_arg="$1"
solution_path="$repo_root/$solution_arg"

if [ ! -f "$solution_path" ]; then
  echo "error: solution not found at $solution_path" >&2
  exit 1
fi

case "$solution_path" in
  *.sln|*.slnx) ;;
  *)
    echo "error: expected a .sln or .slnx file, got $solution_path" >&2
    exit 1
    ;;
esac

solution_name="$(basename "${solution_path%.*}")"
report_dir="$repo_root/ci/results/inspectcode"
report_path="$report_dir/$solution_name.xml"

mkdir -p "$report_dir"
rm -f "$report_path"

cd "$repo_root"

echo "Analyzing $solution_arg with JetBrains InspectCode..."
dotnet jb inspectcode "$solution_path" \
  --build \
  --swea \
  --severity=WARNING \
  --format=Xml \
  --output="$report_path" \
  --no-updates

issues="$(grep -E '<Issue[[:space:]]' "$report_path" || true)"
if [ -z "$issues" ]; then
  echo "JetBrains InspectCode found no Warning or Error issues."
  exit 0
fi

issue_count="$(printf '%s\n' "$issues" | grep -c '^')"

echo
echo "JetBrains InspectCode found $issue_count Warning or Error issue(s):"
printf '%s\n' "$issues" |
  sed -E 's/.*TypeId="([^"]*)".*File="([^"]*)".*Line="([^"]*)".*Message="([^"]*)".*/\2:\3: [\1] \4/'
echo
echo "Full report: $report_path"

exit 1
