#!/usr/bin/env bash
if [ $# -le 0 ]; then
    echo "How to call: ./infrastructure/test.sh http://192.168.0.1:5000"
    exit 1
fi

if [ "$2" = "staging" ]; then
    RUN_STAGING="true"
else
    RUN_STAGING="false"
fi

rm -rf ci/tests/results
docker build -t dd-test-e2e -f ci/tests/e2e.dockerfile . || exit $?
docker run -t --rm \
    -e TZ=America/New_York \
    -e RUN_CONTAINER='true' \
    -e RUN_STAGING=$RUN_STAGING \
    -e ARTIFACTS_PATH='/app/artifacts' \
    -e URL="$1" \
    -v "$(pwd)"/ci/tests/results:/app/artifacts \
    --name dd-test-e2e \
    dd-test-e2e
