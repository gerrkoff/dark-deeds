#!/usr/bin/env bash
if [ $# -le 0 ]; then
    echo "How to call: ./ci/test.sh google.com"
    exit 1
fi

rm -rf ci/results
docker build -t dd-test-load -f ci/apps/tests-load.dockerfile . || exit $?
docker run -t --rm \
    -e DOMAIN="$1" \
    -v "$(pwd)"/ci/results:/app/bin/Release/net5.0/reports \
    --name dd-test-load \
    dd-test-load
