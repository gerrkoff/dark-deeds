#!/usr/bin/env bash
cd ..
rm -rf Deploy/artifacts
docker build -t dark-deeds-test-e2e -f Deploy/dockerfile-test-e2e .
docker rm -f dark-deeds-test-e2e
docker run -v "$(pwd)"/Deploy/artifacts:/app/artifacts --name dark-deeds-test-e2e dark-deeds-test-e2e
docker rm -f dark-deeds-test-e2e
