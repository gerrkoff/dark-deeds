#!/usr/bin/env bash
docker-compose \
    -f infrastructure/prod/docker-compose.yml \
    -p dd_prod \
    down

docker-compose \
    -f infrastructure/prod/docker-compose.yml \
    -p dd_prod \
    rm -v