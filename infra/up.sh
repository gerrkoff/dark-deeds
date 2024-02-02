#!/bin/bash
docker-compose \
  -f infra/docker-compose.yml \
  -p dd \
  up \
  -d --build --remove-orphans # --force-recreate

./code/data/db-update.sh
