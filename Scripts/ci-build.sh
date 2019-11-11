#!/usr/bin/env bash
cd ..
rm -rf CI/artifacts
docker build -t dark-deeds-builder -f CI/build.dockerfile .
docker rm -f dark-deeds-builder
docker run -t \
    -v "$(pwd)"/CI/artifacts:/app/CI/artifacts \
    --name dark-deeds-builder \
    dark-deeds-builder
docker rm -f dark-deeds-builder 
