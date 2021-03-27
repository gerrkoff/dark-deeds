#!/usr/bin/env bash
./infrastructure/run-network.sh
docker rm -f dark-deeds-db
docker run -d \
    -p 5432:5432 \
    -v dark-deeds-psql-volume:/var/lib/postgresql/data \
    -e POSTGRES_PASSWORD=password \
    --network=dark-deeds-nw \
    --name dark-deeds-db \
    postgres:12.0