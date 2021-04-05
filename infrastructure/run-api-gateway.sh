#!/usr/bin/env bash
./infrastructure/run-network.sh
docker build -t dd-api-gateway -f infrastructure/apps/ApiGatewayApp/dockerfile . || exit $?
docker rm -f dd-api-gateway
docker run -d \
    -p 5000:80 \
    --network=dd-network \
    --name dd-api-gateway \
    dd-api-gateway
