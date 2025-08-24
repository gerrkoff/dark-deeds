#!/bin/bash
docker-compose \
  -f infra/docker-compose.yml \
  -p dd \
  logs -f $1
