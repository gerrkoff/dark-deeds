#!/usr/bin/env bash
if [ $# -le 0 ]; then
    echo "How to call: ./infrastructure/test.sh 192.168.0.1"
    exit 1
fi

rm -rf infrastructure/tests/results
docker build -t dd-test-e2e -f infrastructure/tests/e2e.dockerfile . || exit $?
docker run -t -i --rm \
    -e TZ=America/New_York \
    -e RUN_CONTAINER='true' \
    -e ARTIFACTS_PATH='/app/artifacts' \
    -e URL="http://$1:5000" \
    -v "$(pwd)"/infrastructure/tests/results:/app/artifacts \
    --name dd-test-e2e \
    dd-test-e2e
