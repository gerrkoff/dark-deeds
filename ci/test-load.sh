#!/usr/bin/env bash
rm -rf ci/results
docker build -t dd-test-load -f ci/apps/tests-load.dockerfile . || exit $?
docker run -t --rm \
    -v "$(pwd)"/ci/results:/app/bin/Release/net5.0/reports \
    --name dd-test-load \
    dd-test-load
