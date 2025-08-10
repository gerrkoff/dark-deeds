#!/bin/sh
docker build \
    -t gerrkoff/dark-deeds/mcp:latest \
    --platform linux/amd64 \
    -f ./ci/apps/mcp.dockerfile \
    . || exit $?
