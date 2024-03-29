#!/usr/bin/env bash
if [ $# -le 0 ]; then
  echo "How to call: ./ci/test-e2e.sh http://192.168.0.1:5000"
  exit 1
fi

echo
echo "----------- Testing e2e..."

if [ "$2" = "staging" ]; then
  RUN_STAGING="true"
else
  RUN_STAGING="false"
fi

rm -rf ci/results
docker build -t dd-test-e2e -f ci/apps/tests-e2e.dockerfile . || exit $?
docker run -t --rm \
  -e TZ=America/New_York \
  -e RUN_CONTAINER='true' \
  -e RUN_STAGING=$RUN_STAGING \
  -e ARTIFACTS_PATH='/app/artifacts' \
  -e URL="$1" \
  -v "$(pwd)"/ci/results:/app/artifacts \
  --name dd-test-e2e \
  dd-test-e2e || exit $?

echo
echo "----------- Test e2e completed"
