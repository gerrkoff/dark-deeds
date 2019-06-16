#!/usr/bin/env bash

print () {
   printf "\n\e[46m\e[1m    $1    \e[0m\e[0m\n\n"
}

# cleanup
print 'CLEAN'
rm -rf ../run/src
rm -rf ../run/test-results
echo cleaned

# test & build FE
print 'FE: GET DEPS'
cd ../../DarkDeeds.WebClient/
npm install || exit $?

print 'FE: TEST'
npm run test-ci || exit $?

print 'FE: BUILD'
npm run build || exit $?

# test & build BE
print 'BE: TEST'
cd ../DarkDeeds/DarkDeeds.Tests/
dotnet test "--logger:trx;LogFileName=results.trx" --results-directory ../../Deploy/run/test-results || exit $?

print 'BE: SET BUILD VERSION'
cd ../DarkDeeds.Api/
if [ -z "$1" ]
    then
        echo "No BUILD_VERSION provided, skip"
    else
        dotnet ../../Deploy/build/dotnet-setversion/dotnet-setversion.dll vs $BUILD_VERSION
fi

print 'BE: BUILD'
dotnet publish -c Release -o ../../Deploy/run/src || exit $?

# misc
print 'COPY PROD CONFIG'
cd ../../Deploy
cp appsettings.Production.json run/src/
echo copied

print 'SUCCESS'
