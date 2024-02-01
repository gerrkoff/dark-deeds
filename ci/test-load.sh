#!/usr/bin/env bash
if [ $# -le 0 ]; then
  echo "How to call: ./ci/test-load.sh google.com"
  exit 1
fi

echo "----------- Testing load..."

SETTINGS_VOLUME=''
if [ "$2" != "" ]; then
  SETTINGS_VOLUME="-v $(pwd)/$2:/app/appsettings.json:ro"
fi

rm -rf ci/results
docker build -t dd-test-load -f ci/apps/tests-load.dockerfile . || exit $?
docker run -t --rm \
  -e DOMAIN="$1" \
  -v "$(pwd)"/ci/results:/app/bin/Release/net8.0/reports \
  $SETTINGS_VOLUME \
  --name dd-test-load \
  dd-test-load || exit $?

echo "----------- Test load completed"
