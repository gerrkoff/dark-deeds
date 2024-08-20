#!/usr/bin/env bash
if [ $# -le 0 ]; then
  echo "How to call: ./ci/test-e2e.sh http://192.168.0.1:5000"
  exit 1
fi

echo

# https://hub.docker.com/r/selenium/standalone-chrome/tags?page=&page_size=&name=&ordering=last_updated

echo "----------- Removing previous containers and networks..."
docker rm -f dd-test-e2e-chrome
docker network rm dd-test-e2e-network

echo "----------- Starting Selenium Grid..."
docker network create dd-test-e2e-network
docker run -d \
  --network dd-test-e2e-network \
  --platform linux/x86_64 \
  --name dd-test-e2e-chrome \
  selenium/standalone-chrome:127.0-20240813

echo "----------- Waiting for Selenium Grid to start..."
for ((i=5; i>0; i--)); do
  echo "Sleeping for $i seconds..."
  sleep 1
done
echo "----------- GO!"

echo "----------- Running tests..."
rm -rf ci/results
docker build -t dd-test-e2e -f ci/apps/tests-e2e.dockerfile . || exit $?
docker run -t --rm \
  --network dd-test-e2e-network \
  -e TZ=America/New_York \
  -e ARTIFACTS_PATH='/app/artifacts' \
  -e URL="$1" \
  -e SELENIUM_GRID_URL='http://dd-test-e2e-chrome:4444' \
  -v "$(pwd)"/ci/results:/app/artifacts \
  --name dd-test-e2e \
  dd-test-e2e || exit $?

echo "----------- Removing containers and networks..."
docker rm -f dd-test-e2e-chrome
docker network rm dd-test-e2e-network

echo "----------- Test e2e completed"
