#!/usr/bin/env bash
./ci/build.sh
./infrastructure/reup.sh
./infrastructure/test.sh
