#!/usr/bin/env bash
cd ../Deploy/artifacts || exit $?
docker build -t dark-deeds -f dockerfile-run .
docker rm -f dark-deeds
docker run -p 5000:80 --name dark-deeds dark-deeds
