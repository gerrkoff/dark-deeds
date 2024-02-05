#!/bin/bash

echo
echo "----------- Building..."

export BUILD_VERSION=$(./ci/version.sh "$1")
echo BUILD_VERSION=$BUILD_VERSION

docker build \
    -t gerrkoff/dark-deeds:latest \
    -t gerrkoff/dark-deeds:$BUILD_VERSION \
    --build-arg BUILD_VERSION=$BUILD_VERSION \
    -f ./ci/apps/app.dockerfile \
    . || exit $?

echo
echo "----------- Build completed"
