#!/bin/bash

set -e

REPO_URL="https://github.com/gerrkoff/coding-standards"
REPO_RAW_URL="https://raw.githubusercontent.com/gerrkoff/coding-standards/main"

FILES=(
  ".editorconfig"
)

echo "🔄 Syncing configuration files from $REPO_URL"

sync_file() {
  local file_path=$1
  local target_path=${2:-$file_path}

  echo "📄 Syncing $file_path..."

  if curl -fsSL "$REPO_RAW_URL/$file_path" -o "$target_path.tmp" 2>/dev/null; then
    if [ -f "$target_path" ]; then
      if diff -q "$target_path" "$target_path.tmp" > /dev/null 2>&1; then
        echo "   ⏭️  File $target_path unchanged, skipping"
        rm "$target_path.tmp"
      else
        mv "$target_path.tmp" "$target_path"
        echo "   ✅ File $target_path updated"
      fi
    else
      mv "$target_path.tmp" "$target_path"
      echo "   ✅ File $target_path created"
    fi
  else
    echo "   ❌ Failed to download $file_path"
    rm -f "$target_path.tmp"
  fi
}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/.."

for file in "${FILES[@]}"; do
  sync_file "$file"
done

echo ""
echo "✨ Sync complete!"
echo ""
