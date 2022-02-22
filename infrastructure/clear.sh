#!/usr/bin/env bash
docker-compose \
  -f infrastructure/docker-compose.yml \
  -p dd \
  rm -v

docker volume rm dd_nginx-consul-volume
