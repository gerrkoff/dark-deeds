#!/usr/bin/env bash
BRANCH=$(git rev-parse --abbrev-ref HEAD)
BUILD_VERSION=${1:-$BRANCH}

echo BUILD_VERSION=$BUILD_VERSION > infrastructure/prod/.env
docker-compose \
    -f infrastructure/prod/docker-compose.yml \
    -p dd_prod \
    up -d --remove-orphans --force-recreate
