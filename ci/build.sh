#!/usr/bin/env bash

DOCKER_COMPOSE_FILE="ci/docker-compose.yml"
BRANCH=$(git rev-parse --abbrev-ref HEAD)
COMMIT_TIME=$(git show -s --format=%ct)
TIME_FORMATTED=$(date -r $COMMIT_TIME +"%Y%m%d-%H%M%S")
DEPLOY_BRANCH="staging"
DEPLOY_BRANCH="adjust-ci"

if [ "$BRANCH" = "$DEPLOY_BRANCH" ]; then
    BUILD_VERSION="$TIME_FORMATTED"
else
    BUILD_VERSION="$BRANCH"
fi

export BUILD_VERSION=$BUILD_VERSION
echo BUILD_VERSION=$BUILD_VERSION

# docker-compose \
#     -f "${DOCKER_COMPOSE_FILE}" \
#     build || exit $?

# IMAGES=$(cat ${DOCKER_COMPOSE_FILE} | grep 'image: ' | cut -d':' -f 2 | tr -d '"')
# for IMAGE in $IMAGES
# do
#     if [ "$1" = "push" ]; then
#         docker push "${IMAGE}":"${BUILD_VERSION}"

#         if [ "$BRANCH" = "$DEPLOY_BRANCH" ]; then
#             docker tag "${IMAGE}":"${BUILD_VERSION}" "${IMAGE}":latest
#             docker push "${IMAGE}":latest
#         fi
#     fi
# done

# if [ "$BRANCH" = "$DEPLOY_BRANCH" ]; then
    # git tag v$BUILD_VERSION
    # git push --tags || true
# fi
