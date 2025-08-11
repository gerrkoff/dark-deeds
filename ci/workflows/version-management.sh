#!/bin/bash

# Script to manage deployment version tracking using GitHub Repository Variables
# Usage: ./manage-deployment-versions.sh <operation> <repository> <github_token> <version>
# Operations: determine, update, get-previous

set -e

OPERATION="$1"
REPOSITORY="$2"
GITHUB_TOKEN="$3"
NEW_VERSION="$4"

CURRENT_VAR="PROD_VERSION_CURRENT"
PREVIOUS_VAR="PROD_VERSION_PREVIOUS"

# Function to get repository variable
get_variable() {
    local var_name="$1"

    local response=$(curl -s -w "\n%{http_code}" \
        -H "Authorization: Bearer $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github+json" \
        -H "X-GitHub-Api-Version: 2022-11-28" \
        "https://api.github.com/repos/$REPOSITORY/actions/variables/$var_name" 2>&1)

    local body=$(echo "$response" | head -n -1)

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

    # Check if variable exists
    local existing=$(curl -s -w "\n%{http_code}" \
        -H "Authorization: Bearer $GITHUB_TOKEN" \
        -H "Accept: application/vnd.github+json" \
        -H "X-GitHub-Api-Version: 2022-11-28" \
        "https://api.github.com/repos/$REPOSITORY/actions/variables/$var_name" 2>&1)

    local check_http_code=$(echo "$existing" | tail -n1)
    local check_body=$(echo "$existing" | head -n -1)

    if echo "$check_body" | grep -q '"name"'; then
        # Update existing variable
        local update_url="https://api.github.com/repos/$REPOSITORY/actions/variables/$var_name"

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
    else
        # Create new variable
        local create_url="https://api.github.com/repos/$REPOSITORY/actions/variables"

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
    fi
}

case "$OPERATION" in
    "determine")
        if [ -z "$NEW_VERSION" ]; then
            echo "❌ Version required for determine operation"
            echo "Usage: $0 determine <repository> <github_token> <version>"
            exit 1
        fi

        if [ "$NEW_VERSION" = "--" ]; then
            echo "🔄 Rollback requested, getting previous version..."
            DEPLOY_VERSION=$(get_variable "$PREVIOUS_VAR")
            if [ -n "$DEPLOY_VERSION" ]; then
                echo "Rolling back to version: $DEPLOY_VERSION"
            else
                echo "❌ No previous version found for rollback" >&2
                exit 1
            fi
        else
            echo "🚀 Regular deployment requested..."
            DEPLOY_VERSION="$NEW_VERSION"
            echo "Deploying version: $DEPLOY_VERSION"
        fi

        # Output the deployment version for use in GitHub Actions
        echo "version=$DEPLOY_VERSION"
        ;;

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

        echo "Current version: $CURRENT_VERSION"

        if [ -n "$CURRENT_VERSION" ]; then
            echo "Current version: $CURRENT_VERSION"

            # Move current to previous
            echo "📦 Moving current version to previous..."
            set_variable "$PREVIOUS_VAR" "$CURRENT_VERSION"
            echo "✅ Previous version set to: $CURRENT_VERSION"
        else
            echo "No current version found (first deployment)"
        fi

        # # Set new version as current
        # set_variable "$CURRENT_VAR" "$NEW_VERSION"
        # echo "✅ Current version set to: $NEW_VERSION"
        # echo "🎉 Version tracking updated successfully!"
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
        echo "  determine <repo> <token> <version>  - Determine deployment version (use '--' for rollback)"
        echo "  update <repo> <token> <version>     - Move current to previous, set new current"
        echo "  get-previous <repo> <token>         - Get previous deployed version"
        echo ""
        echo "Examples:"
        echo "  $0 determine owner/repo \$GITHUB_TOKEN 20250812-143052"
        echo "  $0 determine owner/repo \$GITHUB_TOKEN --"
        echo "  $0 update owner/repo \$GITHUB_TOKEN 20250812-143052"
        echo "  $0 get-previous owner/repo \$GITHUB_TOKEN"
        exit 1
        ;;
esac
