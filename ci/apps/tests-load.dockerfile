FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY code/tests/DarkDeeds.LoadTests/DarkDeeds.LoadTests.sln /app/DarkDeeds.LoadTests.sln
COPY code/tests/DarkDeeds.LoadTests/DarkDeeds.LoadTests.csproj /app/DarkDeeds.LoadTests.csproj

RUN dotnet restore /app/DarkDeeds.LoadTests.sln

COPY code/tests/DarkDeeds.LoadTests/ /app
COPY .editorconfig /app/.editorconfig

ENTRYPOINT ["dotnet", "test", "--no-restore", "-c", "Release"]
