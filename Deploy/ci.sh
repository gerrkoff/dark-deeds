#!/usr/bin/env bash

cd ..
DIR="$PWD"

cd "$DIR"/DarkDeeds.WebClient/ || exit $?
npm install || exit $?
npm rebuild node-sass || exit $?
npm run test-ci || exit $?
npm run build || exit $?

cd "$DIR"/DarkDeeds/DarkDeeds.Tests/ || exit $?
dotnet test || exit $?

cd "$DIR"/DarkDeeds/DarkDeeds.Api/ || exit $?
dotnet publish || exit $?
