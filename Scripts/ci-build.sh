#!/usr/bin/env bash
rm -rf CI/artifacts
docker build -t dark-deeds-builder -f CI/build.dockerfile .

docker rm -f dark-deeds-builder
docker run -t \
    -v "$(pwd)"/CI/artifacts:/app/CI/artifacts \
    --name dark-deeds-builder \
    dark-deeds-builder
docker rm -f dark-deeds-builder 

cd Scripts
VERSION=$(sh version-get.sh)
cd ..

cd CI/artifacts || exit $?
docker build \
    -f run.dockerfile \
    -t gerrkoff/dark-deeds:$VERSION \
    -t gerrkoff/dark-deeds:latest \
    . || exit $?
docker push gerrkoff/dark-deeds:$VERSION
docker push gerrkoff/dark-deeds:latest
