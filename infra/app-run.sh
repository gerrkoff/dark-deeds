#!/bin/bash
TAG=${1:-latest}
docker run --rm \
  -p 5000:8080 \
  --name dark-deeds \
  -v "$(pwd)/code/backend/DD.App/appsettings.Development.json":/app/appsettings.Production.json \
  gerrkoff/dark-deeds:$TAG
