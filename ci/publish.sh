#!/bin/bash

./ci/build.sh

echo "----------- Publishing..."

export BUILD_VERSION=$(./ci/version.sh "$1")
echo BUILD_VERSION=$BUILD_VERSION

docker push gerrkoff/dark-deeds:latest || exit $?
docker push gerrkoff/dark-deeds:$BUILD_VERSION  || exit $?

echo "----------- Publish completed"
