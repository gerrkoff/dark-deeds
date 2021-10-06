#!/usr/bin/env bash
cp DarkDeeds/DarkDeeds.Api/settings/appsettings.Development.json DarkDeeds/DarkDeeds.Api/settings/appsettings.Production.json || exit $?
docker rm -f dark-deeds
docker run -t \
    -p 5000:80 \
    -v "$(pwd)"/DarkDeeds/DarkDeeds.Api/settings:/app/settings \
    --name dark-deeds \
    gerrkoff/dark-deeds:latest
