#!/usr/bin/env bash
./infrastructure/run-network.sh
docker build -t dark-deeds-task-service-app -f infrastructure/apps/TaskServiceApp/dockerfile . || exit $?
docker rm -f dark-deeds-task-service-app
docker run -d \
    -p 5001:80 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --network=dark-deeds-nw \
    --name dark-deeds-task-service-app \
    dark-deeds-task-service-app
