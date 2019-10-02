#!/usr/bin/env bash
cd ..
rm -rf CI/artifacts
docker build -t dark-deeds-test-e2e -f CI/dockerfile-test-e2e .
docker rm -f dark-deeds-test-e2e
docker run \
    -e TZ=America/New_York \
    -e RUN_CONTAINER='true' \
    -e ARTIFACTS_PATH='/app/artifacts' \
    -e API_URL='http://localhost:5000' \
    -e URL='http://localhost:3000' \
    -e USERNAME='qqq' \
    -e PASSWORD='qqq' \
    -v "$(pwd)"/CI/artifacts:/app/artifacts \
    --name dark-deeds-test-e2e \
    dark-deeds-test-e2e
docker rm -f dark-deeds-test-e2e
