#!/usr/bin/env bash
./infrastructure/run-network.sh
docker build -t dd-auth-service -f infrastructure/apps/AuthServiceApp/dockerfile . || exit $?
docker rm -f dd-auth-service
docker run -d \
    -p 5002:80 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --network=dd-network \
    --name dd-auth-service \
    dd-auth-service
