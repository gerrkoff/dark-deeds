#!/usr/bin/env bash
./infrastructure/run-network.sh
docker build -t dark-deeds-auth-service-app -f infrastructure/apps/AuthServiceApp/dockerfile . || exit $?
docker rm -f dark-deeds-auth-service-app
docker run -d \
    -p 5002:80 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --network=dark-deeds-nw \
    --name dark-deeds-auth-service-app \
    dark-deeds-auth-service-app
