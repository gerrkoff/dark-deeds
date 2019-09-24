#!/usr/bin/env bash
cd ..
rm -rf Deploy/artifacts
docker build -t dark-deeds-builder -f Deploy/dockerfile-build .
docker rm -f dark-deeds-builder
docker run -v "$(pwd)"/Deploy/artifacts:/app/Deploy/artifacts --name dark-deeds-builder dark-deeds-builder
docker rm -f dark-deeds-builder 
