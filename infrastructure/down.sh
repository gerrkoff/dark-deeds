#!/usr/bin/env bash
docker-compose \
  -f infrastructure/docker-compose.yml \
  -p dd \
  down -v
