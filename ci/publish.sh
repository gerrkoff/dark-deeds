#!/bin/bash

export BUILD_VERSION=$(./ci/version.sh "$1")
echo BUILD_VERSION=$BUILD_VERSION

./ci/build.sh

docker push \
  gerrkoff/dark-deeds:latest \
  gerrkoff/translations-history:$BUILD_VERSION || exit $?
