#!/bin/bash

# Script to trigger a Drone CI job and return the build number
# Usage: ./trigger-drone-job.sh <operation> <version> <branch> <drone_server> <drone_token> <repository>

set -e

OPERATION="$1"
VERSION="$2"
BRANCH="$3"
DRONE_SERVER="$4"
DRONE_TOKEN="$5"
REPOSITORY="$6"

if [ -z "$OPERATION" ] || [ -z "$VERSION" ] || [ -z "$BRANCH" ] || [ -z "$DRONE_SERVER" ] || [ -z "$DRONE_TOKEN" ] || [ -z "$REPOSITORY" ]; then
    echo "Usage: $0 <operation> <version> <branch> <drone_server> <drone_token> <repository>"
    echo "Example: $0 deploy-staging v1.2.3 master https://drone.example.com your_token owner/repo"
    exit 1
fi

DRONE_URL="$DRONE_SERVER/api/repos/$REPOSITORY/builds?branch=$BRANCH&operation=$OPERATION&version=$VERSION"

echo "Triggering Drone CI job..."
echo "Operation: $OPERATION"
echo "Version: $VERSION"
echo "Branch: $BRANCH"
echo "Repository: $REPOSITORY"
echo "Drone URL: $DRONE_URL"

RESPONSE=$(curl -X POST \
    -H "Authorization: Bearer $DRONE_TOKEN" \
    -H "Content-Type: application/json" \
    "$DRONE_URL")

echo "Drone CI Response:"
echo "$RESPONSE"

# Extract build number from response
BUILD_NUMBER=$(echo "$RESPONSE" | grep -o '"number":[0-9]*' | head -1 | cut -d':' -f2)

if [ -z "$BUILD_NUMBER" ]; then
    echo "❌ Failed to extract build number from Drone response"
    echo "Response was: $RESPONSE"
    exit 1
fi

echo "✅ Job triggered successfully!"
echo "Build number: $BUILD_NUMBER"

# Return build number for use in GitHub Actions
echo "build_number=$BUILD_NUMBER"
