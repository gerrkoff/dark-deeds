#!/usr/bin/env bash
echo IP=$1 > infrastructure/.env
cat infrastructure/.env
docker-compose \
    -f infrastructure/docker-compose.yml \
    -p dd \
    up \
    -d --build --remove-orphans # --force-recreate