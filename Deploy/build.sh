#!/usr/bin/env bash

print () {
   printf "\n\e[46m\e[1m    $1    \e[0m\e[0m\n\n"
}

cd ..
DIR="$PWD"

# cleanup
print 'CLEAN'
rm -rf "$DIR"/Deploy/artifacts
echo cleaned

# test & build FE
print 'FE: GET DEPS'
cd "$DIR"/DarkDeeds.WebClient/ || exit $?
npm install || exit $?
npm rebuild node-sass || exit $?

print 'FE: TEST'
npm run test-ci || exit $?

print 'FE: BUILD'
npm run build || exit $?

# test & build BE
print 'BE: TEST'
cd "$DIR"/DarkDeeds/DarkDeeds.Tests/ || exit $?
dotnet test "--logger:trx;LogFileName=results.trx" --results-directory "$DIR"/Deploy/artifacts/test-results || exit $?

print 'BE: SET BUILD VERSION'
cd "$DIR"/DarkDeeds/DarkDeeds.Api/ || exit $?
if [ -z "$1" ]
    then
        echo "No BUILD_VERSION provided, skip"
    else
        dotnet "$DIR"/Deploy/dotnet-setversion/dotnet-setversion.dll vs $1
fi

print 'BE: BUILD'
dotnet publish -c Release -o "$DIR"/Deploy/artifacts/src || exit $?

# misc
print 'COPY ADDITIONAL FILES'
cd "$DIR" || exit $?
cp appsettings.Production.json Deploy/artifacts/src || exit $?
cp Deploy/dockerfile-run Deploy/artifacts/ || exit $?
echo copied

print 'SUCCESS'
