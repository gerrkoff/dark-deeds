#!/bin/bash

BRANCH=$(git rev-parse --abbrev-ref HEAD | sed "s/\./-/g" | sed "s/\//-/g" | sed "s/_/-/g" )
COMMIT_TIME=$(git show -s --format=%ct)
DATE_CMD=${1:-'date -r '}
TIME_FORMATTED=$($DATE_CMD$COMMIT_TIME +"%Y%m%d-%H%M%S")
DEPLOY_BRANCH="staging"

if [ "$BRANCH" == "$DEPLOY_BRANCH" ]; then
    echo $TIME_FORMATTED
else
    echo $BRANCH
fi
