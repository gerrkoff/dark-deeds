#!/bin/sh

# Cross-platform version script for Docker image tagging
# Works on macOS, Linux, and other POSIX-compatible systems

# Get current branch name and sanitize it for Docker tag format
# Use branch name passed as parameter if available, otherwise fallback to git
RAW_BRANCH="${1:-$(git rev-parse --abbrev-ref HEAD)}"

# Sanitize branch name for Docker tag format
# Кeplace non-allowed characters with -
BRANCH=$(echo "$RAW_BRANCH" | sed 's/[^a-zA-Z0-9._-]/-/g')

# Get commit timestamp in UTC seconds since epoch
COMMIT_TIME=$(git show -s --format=%ct)

# Define staging branch name
DEPLOY_BRANCH="staging"

# Function to format timestamp in a cross-platform way using only POSIX utilities
format_timestamp() {
    timestamp=$1

    # Try different date command formats based on the system
    # First try GNU date format (Linux)
    if date -d "@$timestamp" -u +"%Y%m%d-%H%M%S" 2>/dev/null; then
        return 0
    # Then try BSD date format (macOS)
    elif date -r "$timestamp" -u +"%Y%m%d-%H%M%S" 2>/dev/null; then
        return 0
    # Fallback: use awk for timestamp conversion if it supports strftime
    elif awk "BEGIN { t = $timestamp; print strftime(\"%Y%m%d-%H%M%S\", t, 1) }" 2>/dev/null | grep -E '^[0-9]{8}-[0-9]{6}$' >/dev/null; then
        awk "BEGIN { t = $timestamp; print strftime(\"%Y%m%d-%H%M%S\", t, 1) }"
        return 0
    else
        # Final fallback: use a simpler approach with just the timestamp
        # This isn't a perfect date format but ensures the script doesn't fail
        echo "ts$timestamp"
        return 0
    fi
}

# Generate version based on branch
if [ "$BRANCH" = "$DEPLOY_BRANCH" ]; then
    # For staging branch: use formatted commit timestamp
    VERSION=$(format_timestamp "$COMMIT_TIME")
    echo "$VERSION"
else
    # For other branches: use sanitized branch name
    echo "$BRANCH"
fi
