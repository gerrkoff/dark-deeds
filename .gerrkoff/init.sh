#!/bin/bash
# Usage: curl -fsSL "https://raw.githubusercontent.com/gerrkoff/coding-standards/main/.gerrkoff/init.sh?$(date +%s)" | bash

set -e

REPO_URL="https://github.com/gerrkoff/coding-standards"
TEMP_DIR=$(mktemp -d)
TARGET_DIR="$PWD"

DIRECTORIES=(
  ".devcontainer"
  ".gerrkoff"
)

echo "🚀 Initializing environment from $REPO_URL"

copy_directory() {
  local dir_name=$1
  local source_path=$2
  local target_dir="$TARGET_DIR/$dir_name"

  echo "📦 Syncing $dir_name..."

  cd "$TEMP_DIR"
  rm -rf temp-repo

  git clone --depth 1 --filter=blob:none --sparse "$REPO_URL" temp-repo 2>/dev/null
  cd temp-repo
  git sparse-checkout set "$source_path"

  if [ -d "$source_path" ]; then
    mkdir -p "$target_dir"

    if command -v rsync &> /dev/null; then
      if rsync -a --exclude='.git' "$source_path/" "$target_dir/"; then
        echo "✅ $dir_name synced"
      else
        echo "❌ Failed to sync $dir_name with rsync"
        return 1
      fi
    else
      if [ "$(cd "$source_path" && pwd)" != "$(cd "$target_dir" && pwd)" ]; then
        if cp -rf "$source_path/." "$target_dir/"; then
          echo "✅ $dir_name synced"
        else
          echo "❌ Failed to sync $dir_name with cp"
          return 1
        fi
      else
        echo "⚠️  Source and target are the same, skipping $dir_name"
      fi
    fi
  else
    echo "❌ Failed to find $source_path in repository"
    return 1
  fi
}

for dir in "${DIRECTORIES[@]}"; do
  copy_directory "$dir" "$dir"
done

rm -rf "$TEMP_DIR"

echo ""
echo "✨ Initialization complete!"
echo ""
echo "📝 Next steps: Use .gerrkoff/sync.sh to sync configuration files"
echo ""
