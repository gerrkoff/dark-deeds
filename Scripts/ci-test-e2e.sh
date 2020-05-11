#!/usr/bin/env bash
cd ..
rm -rf CI/artifacts
docker build -t dark-deeds-test-e2e -f CI/test-e2e.dockerfile . || exit $?
docker rm -f dark-deeds-test-e2e
docker run -t \
    -e TZ=America/New_York \
    -e RUN_CONTAINER='true' \
    -e ARTIFACTS_PATH='/app/artifacts' \
    -e URL='https://dark-deeds.grkf.ru' \
    -e USERNAME='' \
    -e PASSWORD='' \
    -v "$(pwd)"/CI/artifacts:/app/artifacts \
    --name dark-deeds-test-e2e \
    dark-deeds-test-e2e
docker rm -f dark-deeds-test-e2e
