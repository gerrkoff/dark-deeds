#!/usr/bin/env bash
./infrastructure/run-network.sh
docker build -t dd-telegram-client -f infrastructure/apps/TelegramClientApp/dockerfile . || exit $?
docker rm -f dd-telegram-client
docker run -d \
    -p 5003:80 \
    --network=dd-network \
    --name dd-telegram-client \
    dd-telegram-client
