FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY code/tests/DarkDeeds.E2eTests/DarkDeeds.E2eTests.sln /app/DarkDeeds.E2eTests.sln
COPY code/tests/DarkDeeds.E2eTests/DarkDeeds.E2eTests.csproj /app/DarkDeeds.E2eTests.csproj

RUN dotnet restore /app/DarkDeeds.E2eTests.sln

COPY code/tests/DarkDeeds.E2eTests/ /app
COPY .editorconfig /app/.editorconfig

ENTRYPOINT ["dotnet", "test", "--no-restore", "-c", "Release", "--results-directory", "artifacts", "--logger:trx;LogFileName=results.trx", "-v", "normal"]
