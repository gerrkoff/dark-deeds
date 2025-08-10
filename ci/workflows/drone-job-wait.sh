#!/bin/bash

# Script to wait for a Drone CI build to complete
# Usage: ./wait-for-drone.sh <build_number> <drone_server> <drone_token> <repository>

set -e

BUILD_NUMBER="$1"
DRONE_SERVER="$2"
DRONE_TOKEN="$3"
REPOSITORY="$4"

if [ -z "$BUILD_NUMBER" ] || [ -z "$DRONE_SERVER" ] || [ -z "$DRONE_TOKEN" ] || [ -z "$REPOSITORY" ]; then
    echo "Usage: $0 <build_number> <drone_server> <drone_token> <repository>"
    echo "Example: $0 123 https://drone.example.com your_token owner/repo"
    exit 1
fi

echo "Waiting for Drone CI build $BUILD_NUMBER to complete..."

API_URL="$DRONE_SERVER/api/repos/$REPOSITORY/builds/$BUILD_NUMBER"
echo "Drone API URL: $API_URL"

# Wait up to 10 minutes (600 seconds)
MAX_WAIT=600
WAIT_TIME=0
SLEEP_INTERVAL=5

while [ $WAIT_TIME -lt $MAX_WAIT ]; do
    # Get build status
    STATUS_RESPONSE=$(curl -s \
        -H "Authorization: Bearer $DRONE_TOKEN" \
        "$API_URL")

    # echo "Build details:"
    # echo "$STATUS_RESPONSE"

    # Parse status from JSON response using jq for proper JSON parsing
    STATUS=$(echo "$STATUS_RESPONSE" | jq -r '.status // "unknown"')

    echo "Build status: $STATUS (waited ${WAIT_TIME}s)"

    case "$STATUS" in
        "success")
            echo "✅ Drone CI build completed successfully!"
            exit 0
            ;;
        "failure"|"error"|"killed")
            echo "❌ Drone CI build failed with status: $STATUS"
            exit 1
            ;;
        "running"|"pending"|"waiting_on_dependencies")
            echo "⏳ Build is still $STATUS, waiting..."
            ;;
        "skipped")
            echo "⏭️  Build was skipped"
            exit 1
            ;;
        "")
            echo "⚠️  Empty status response, build might not exist or API error"
            exit 1
            ;;
        *)
            echo "❓ Unknown status: $STATUS"
            exit 1
            ;;
    esac

    sleep $SLEEP_INTERVAL
    WAIT_TIME=$((WAIT_TIME + SLEEP_INTERVAL))
done

echo "❌ Timeout: Build did not complete within $MAX_WAIT seconds"
exit 1
