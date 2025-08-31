#!/bin/bash
export URL="http://host.docker.internal:3000"
export BE_URL="http://localhost:5000"
export SELENIUM_GRID_URL="http://localhost:4444/"
export CONTAINER="true"

dotnet test -v normal ./code/tests/DarkDeeds.E2eTests/DarkDeeds.E2eTests.csproj
