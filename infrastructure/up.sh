#!/usr/bin/env bash
docker-compose \
    -f infrastructure/docker-compose.yml \
    -p dd \
    up \
    -d --build --remove-orphans # --force-recreate