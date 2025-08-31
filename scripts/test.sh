#!/bin/bash
export URL="http://host.docker.internal:3000"
if curl -s --connect-timeout 2 http://host.docker.internal:5000 > /dev/null 2>&1; then
    export BE_URL="http://host.docker.internal:5000"
else
    export BE_URL="http://localhost:5000"
fi
export SELENIUM_GRID_URL="http://localhost:4444/"
export CONTAINER="true"

dotnet test -v normal ./code/tests/DarkDeeds.E2eTests/DarkDeeds.E2eTests.csproj
