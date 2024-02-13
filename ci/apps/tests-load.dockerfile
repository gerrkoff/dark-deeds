FROM mcr.microsoft.com/dotnet/sdk:8.0

COPY code/tests/DarkDeeds.LoadTests/ /app
COPY .editorconfig /app/.editorconfig
WORKDIR /app

ENTRYPOINT ["dotnet", "test", "-c", "Release"]
