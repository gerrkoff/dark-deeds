#!/usr/bin/env bash
./ci/build.sh
./infrastructure/reup.sh
sleep 10
./infrastructure/test.sh
