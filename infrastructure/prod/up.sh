#!/usr/bin/env bash
BRANCH=$(git rev-parse --abbrev-ref HEAD)
BUILD_VERSION=${1:-$BRANCH}

export BUILD_VERSION=$BUILD_VERSION
echo "BUILD_VERSION=$BUILD_VERSION"

docker-compose \
    -f infrastructure/prod/docker-compose.yml \
    pull

docker-compose \
    -f infrastructure/prod/docker-compose.yml \
    -p dd_prod \
    up -d --remove-orphans --force-recreate
