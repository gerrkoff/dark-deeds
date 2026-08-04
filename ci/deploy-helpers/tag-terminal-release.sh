#!/usr/bin/env bash

set -euo pipefail

if [ "$#" -ne 1 ]; then
    echo "Usage: ci/deploy-helpers/tag-terminal-release.sh <staging-commit>" >&2
    exit 2
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/../.." && pwd)"
cd "$repo_root"

commit="$(git rev-parse --verify "$1^{commit}")"
remote_staging_line="$(git ls-remote --exit-code origin refs/heads/staging)"
remote_staging="${remote_staging_line%%$'\t'*}"

if [ "$commit" != "$remote_staging" ]; then
    echo "error: commit $commit is not the current origin/staging commit $remote_staging" >&2
    exit 1
fi

echo
read -r -p "Terminal version (for example 1.2.0; leave empty to skip): " version
if [ -z "$version" ]; then
    echo "Terminal release skipped."
    exit 0
fi

if [[ ! "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+(-[0-9A-Za-z.-]+)?(\+[0-9A-Za-z.-]+)?$ ]]; then
    echo "error: terminal version must be a semantic version such as 1.2.0 or 1.2.0-beta.1" >&2
    exit 2
fi

tag="dd-terminal-v$version"
remote_tag="$(git ls-remote --tags origin "refs/tags/$tag" "refs/tags/$tag^{}")"
if [ -n "$remote_tag" ]; then
    echo "error: tag $tag already exists on origin" >&2
    exit 1
fi

if git rev-parse --quiet --verify "refs/tags/$tag" >/dev/null; then
    existing_commit="$(git rev-list -n 1 "$tag")"
    if [ "$existing_commit" != "$commit" ]; then
        echo "error: local tag $tag points to $existing_commit instead of $commit" >&2
        exit 1
    fi
else
    git tag --annotate "$tag" "$commit" --message "Dark Deeds terminal client $version"
fi

git push origin "refs/tags/$tag"
echo "Created terminal release tag $tag at $commit."
