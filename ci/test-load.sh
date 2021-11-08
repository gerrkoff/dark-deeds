#!/usr/bin/env bash
if [ $# -le 0 ]; then
    echo "How to call: ./ci/test.sh google.com"
    exit 1
fi

rm -rf ci/results
docker build -t dd-test-load -f ci/apps/tests-load.dockerfile . || exit $?
docker run -t --rm \
    -e DOMAIN="$1" \
    -e TEST_TIME=$2 \
    -e TEST1_RPS=$3 \
    -e TEST2_RPS=$4 \
    -e TEST3_RPS=$5 \
    -v "$(pwd)"/ci/results:/app/bin/Release/net5.0/reports \
    --name dd-test-load \
    dd-test-load
