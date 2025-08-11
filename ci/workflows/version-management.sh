#!/bin/bash

# Script to manage deployment version tracking using GitHub Repository Variables
# Usage: ./manage-deployment-versions.sh <operation> <repository> <github_token> <new_version>
# Operations: update, get-previous

set -e

OPERATION="$1"
REPOSITORY="$2"
GITHUB_TOKEN="$3"
NEW_VERSION="$4"

CURRENT_VAR="PROD_VERSION_CURRENT"
PREVIOUS_VAR="PROD_VERSION_PREVIOUS"

# Debug information
echo "🐛 [DEBUG] Script started with parameters:" >&2
echo "🐛 [DEBUG] Operation: $OPERATION" >&2
echo "🐛 [DEBUG] Repository: $REPOSITORY" >&2
echo "🐛 [DEBUG] Token length: ${#GITHUB_TOKEN} chars" >&2
echo "🐛 [DEBUG] New version: $NEW_VERSION" >&2
echo "🐛 [DEBUG] Current var name: $CURRENT_VAR" >&2
echo "🐛 [DEBUG] Previous var name: $PREVIOUS_VAR" >&2

# Function to get repository variable
get_variable() {
    local var_name="$1"
    echo "🔍 [DEBUG] Getting repository variable: $var_name" >&2
    echo "🔍 [DEBUG] Repository: $REPOSITORY" >&2
    echo "🔍 [DEBUG] URL: https://api.github.com/repos/$REPOSITORY/actions/variables/$var_name" >&2

    local response=$(curl -s -w "\n%{http_code}" \
        -H "Authorization: Bearer $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github+json" \
        -H "X-GitHub-Api-Version: 2022-11-28" \
        "https://api.github.com/repos/$REPOSITORY/actions/variables/$var_name" 2>&1)

    local http_code=$(echo "$response" | tail -n1)
    local body=$(echo "$response" | head -n -1)

    echo "🔍 [DEBUG] HTTP Status: $http_code" >&2
    echo "🔍 [DEBUG] Response body: $body" >&2

    if echo "$body" | grep -q '"value"'; then
        echo "$body" | grep -o '"value":"[^"]*"' | cut -d'"' -f4
    else
        echo ""
    fi
}

# Function to set repository variable
set_variable() {
    local var_name="$1"
    local var_value="$2"

    echo "📝 [DEBUG] Setting repository variable: $var_name = $var_value" >&2
    echo "📝 [DEBUG] Repository: $REPOSITORY" >&2

    # Check if variable exists
    echo "📝 [DEBUG] Checking if variable exists..." >&2
    local existing=$(curl -s -w "\n%{http_code}" \
        -H "Authorization: Bearer $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github+json" \
        -H "X-GitHub-Api-Version: 2022-11-28" \
        "https://api.github.com/repos/$REPOSITORY/actions/variables/$var_name" 2>&1)

    local check_http_code=$(echo "$existing" | tail -n1)
    local check_body=$(echo "$existing" | head -n -1)

    echo "📝 [DEBUG] Check HTTP Status: $check_http_code" >&2
    echo "📝 [DEBUG] Check Response body: $check_body" >&2

    if echo "$check_body" | grep -q '"name"'; then
        # Update existing variable
        echo "📝 [DEBUG] Updating existing variable..." >&2
        local update_url="https://api.github.com/repos/$REPOSITORY/actions/variables/$var_name"
        echo "📝 [DEBUG] Update URL: $update_url" >&2

        local update_response=$(curl -s -w "\n%{http_code}" \
            -X PATCH \
            -H "Authorization: Bearer $GITHUB_TOKEN" \
            -H "Accept: application/vnd.github+json" \
            -H "X-GitHub-Api-Version: 2022-11-28" \
            -H "Content-Type: application/json" \
            "$update_url" \
            -d "{\"name\":\"$var_name\",\"value\":\"$var_value\"}" 2>&1)

        local update_http_code=$(echo "$update_response" | tail -n1)
        local update_body=$(echo "$update_response" | head -n -1)

        echo "📝 [DEBUG] Update HTTP Status: $update_http_code" >&2
        echo "📝 [DEBUG] Update Response body: $update_body" >&2
    else
        # Create new variable
        echo "📝 [DEBUG] Creating new variable..." >&2
        local create_url="https://api.github.com/repos/$REPOSITORY/actions/variables"
        echo "📝 [DEBUG] Create URL: $create_url" >&2

        local create_response=$(curl -s -w "\n%{http_code}" \
            -X POST \
            -H "Authorization: Bearer $GITHUB_TOKEN" \
            -H "Accept: application/vnd.github+json" \
            -H "X-GitHub-Api-Version: 2022-11-28" \
            -H "Content-Type: application/json" \
            "$create_url" \
            -d "{\"name\":\"$var_name\",\"value\":\"$var_value\"}" 2>&1)

        local create_http_code=$(echo "$create_response" | tail -n1)
        local create_body=$(echo "$create_response" | head -n -1)

        echo "📝 [DEBUG] Create HTTP Status: $create_http_code" >&2
        echo "📝 [DEBUG] Create Response body: $create_body" >&2
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
