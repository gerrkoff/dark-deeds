#!/usr/bin/env bash

set -euo pipefail

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
repo_root="$(cd "$script_dir/.." && pwd)"
cd "$repo_root"

if [ -n "$(git status --porcelain)" ]; then
    echo Current branch is not clean
    exit 1
fi

echo Current branch is clean, move on
echo "Confirm merging to staging:"
read -r

git checkout master
git pull
git checkout staging
git pull
git merge master

git push
staging_commit="$(git rev-parse HEAD)"
git checkout master

echo "Staging deployment commit: $staging_commit"
"$script_dir/deploy-helpers/tag-terminal-release.sh" "$staging_commit"

echo
echo "()___)____________)   Successfully merged to staging"
echo
