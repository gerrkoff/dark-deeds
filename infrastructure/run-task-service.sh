#!/usr/bin/env bash
./infrastructure/run-network.sh
docker build -t dd-task-service -f infrastructure/apps/TaskServiceApp/dockerfile . || exit $?
docker rm -f dd-task-service
docker run -d \
    -p 5001:80 \
    --network=dd-network \
    --name dd-task-service \
    dd-task-service
