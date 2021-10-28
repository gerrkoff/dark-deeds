#!/usr/bin/env bash
export IP=$1

docker-compose \
    -f infrastructure/docker-compose.yml \
    -p dd \
    up \
    -d --build --remove-orphans # --force-recreate
