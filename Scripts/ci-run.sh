#!/usr/bin/env bash
cp ../DarkDeeds/DarkDeeds.Api/appsettings.Development.json ../CI/artifacts/src/settings/appsettings.Production.json || exit $?
cd ../CI/artifacts || exit $?
docker build -t dark-deeds -f run.dockerfile .
docker rm -f dark-deeds
docker run -t \
    -p 5000:80 \
    --name dark-deeds \
    dark-deeds
