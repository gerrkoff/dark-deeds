#!/bin/bash
COMMIT_TIME=$(git show -s --format=%ct)
BUILD_VERSION=$(date -r $COMMIT_TIME +"%Y%m%d-%H%M%S")

docker build \
    -t gerrkoff/dark-deeds-mcp:latest \
    --build-arg BUILD_VERSION=$BUILD_VERSION \
    --platform linux/amd64 \
    -f ./code/mcp/dockerfile \
    . || exit $?

echo VERSION=$BUILD_VERSION

docker push gerrkoff/dark-deeds-mcp:latest || exit $?

echo PUBLISHED
