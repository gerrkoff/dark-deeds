#!/bin/bash

echo "----------- Publishing..."

export BUILD_VERSION=$(./ci/version.sh "$1")
echo BUILD_VERSION=$BUILD_VERSION

./ci/build.sh

docker push \
  gerrkoff/dark-deeds:latest \
  gerrkoff/dark-deeds:$BUILD_VERSION || exit $?

echo "----------- Publish completed"
