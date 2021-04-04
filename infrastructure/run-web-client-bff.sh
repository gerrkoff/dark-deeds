#!/usr/bin/env bash
./infrastructure/run-network.sh
docker build -t dd-web-client-bff -f infrastructure/apps/WebClientBffApp/dockerfile . || exit $?
docker rm -f dd-web-client-bff
docker run -d \
    -p 5000:80 \
    -e ASPNETCORE_ENVIRONMENT=Development \
    --network=dd-network \
    --name dd-web-client-bff \
    dd-web-client-bff
