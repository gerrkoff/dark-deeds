#!/usr/bin/env bash
./infrastructure/run-network.sh
docker build -t dd-web-client -f infrastructure/apps/WebClientApp/dockerfile . || exit $?
docker rm -f dd-web-client
docker run -d \
    -p 3000:3000 \
    --network=dd-network \
    --name dd-web-client \
    dd-web-client
