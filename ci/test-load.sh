#!/usr/bin/env bash
if [ $# -le 0 ]; then
    echo "How to call: ./ci/test.sh google.com"
    exit 1
fi

SETTINGS_VOLUME=''
if [ "$2" != "" ]; then
    SETTINGS_VOLUME="-v $(pwd)/$2:/app/appsettings.json:ro"
fi

rm -rf ci/results
docker build -t dd-test-load -f ci/apps/tests-load.dockerfile . || exit $?
docker rm -f dd-test-load
docker run -t \
    -e DOMAIN="$1" \
    -v "$(pwd)"/ci/results:/app/bin/Release/net5.0/reports \
    $SETTINGS_VOLUME \
    --name dd-test-load \
    dd-test-load
