#!/bin/bash

# Script to manage deployment version tracking using GitHub Environment Variables for Prod
# Usage: ./manage-deployment-versions.sh <operation> <repository> <github_token> <new_version>
# Operations: update, get-previous

set -e

OPERATION="$1"
REPOSITORY="$2"
GITHUB_TOKEN="$3"
NEW_VERSION="$4"

ENVIRONMENT_NAME="prod"
CURRENT_VAR="VERSION_CURRENT"
PREVIOUS_VAR="VERSION_PREVIOUS"

# Function to get environment variable
get_variable() {
    local var_name="$1"
    local response=$(curl -s \
        -H "Authorization: Bearer $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github+json" \
        -H "X-GitHub-Api-Version: 2022-11-28" \
        "https://api.github.com/repos/$REPOSITORY/environments/$ENVIRONMENT_NAME/variables/$var_name" 2>/dev/null)

    if echo "$response" | grep -q '"value"'; then
        echo "$response" | grep -o '"value":"[^"]*"' | cut -d'"' -f4
    else
        echo ""
    fi
}

# Function to set environment variable
set_variable() {
    local var_name="$1"
    local var_value="$2"

    # Check if variable exists
    local existing=$(curl -s \
        -H "Authorization: Bearer $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github+json" \
        -H "X-GitHub-Api-Version: 2022-11-28" \
        "https://api.github.com/repos/$REPOSITORY/environments/$ENVIRONMENT_NAME/variables/$var_name" 2>/dev/null)

    if echo "$existing" | grep -q '"name"'; then
        # Update existing variable
        curl -s \
            -X PATCH \
            -H "Authorization: Bearer $GITHUB_TOKEN" \
            -H "Accept: application/vnd.github+json" \
            -H "X-GitHub-Api-Version: 2022-11-28" \
            "https://api.github.com/repos/$REPOSITORY/environments/$ENVIRONMENT_NAME/variables/$var_name" \
            -d "{\"name\":\"$var_name\",\"value\":\"$var_value\"}" >/dev/null
    else
        # Create new variable
        curl -s \
            -X POST \
            -H "Authorization: Bearer $GITHUB_TOKEN" \
            -H "Accept: application/vnd.github+json" \
            -H "X-GitHub-Api-Version: 2022-11-28" \
            "https://api.github.com/repos/$REPOSITORY/environments/$ENVIRONMENT_NAME/variables" \
            -d "{\"name\":\"$var_name\",\"value\":\"$var_value\"}" >/dev/null
    fi
}

case "$OPERATION" in
    "update")
        if [ -z "$NEW_VERSION" ]; then
            echo "❌ Version required for update operation"
            echo "Usage: $0 update <repository> <github_token> <version>"
            exit 1
        fi

        echo "🔄 Updating deployment versions..."
        echo "New version: $NEW_VERSION"

        # Get current version
        CURRENT_VERSION=$(get_variable "$CURRENT_VAR")
        if [ -n "$CURRENT_VERSION" ]; then
            echo "Current version: $CURRENT_VERSION"

            # Move current to previous
            echo "📦 Moving current version to previous..."
            set_variable "$PREVIOUS_VAR" "$CURRENT_VERSION"
            echo "✅ Previous version set to: $CURRENT_VERSION"
        else
            echo "No current version found (first deployment)"
        fi

        # Set new version as current
        set_variable "$CURRENT_VAR" "$NEW_VERSION"
        echo "✅ Current version set to: $NEW_VERSION"
        echo "🎉 Version tracking updated successfully!"
        ;;

    "get-previous")
        PREVIOUS_VERSION=$(get_variable "$PREVIOUS_VAR")
        if [ -n "$PREVIOUS_VERSION" ]; then
            echo "$PREVIOUS_VERSION"
        else
            echo "❌ No previous version found" >&2
            exit 1
        fi
        ;;

    *)
        echo "Usage: $0 <operation> <repository> <github_token> [version]"
        echo ""
        echo "Operations:"
        echo "  update <repo> <token> <version>  - Move current to previous, set new current"
        echo "  get-previous <repo> <token>      - Get previous deployed version"
        echo ""
        echo "Examples:"
        echo "  $0 update owner/repo \$GITHUB_TOKEN 20250812-143052"
        echo "  $0 get-previous owner/repo \$GITHUB_TOKEN"
        exit 1
        ;;
esac
