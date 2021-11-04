FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY code/tests/DarkDeeds.LoadTests/ /app
WORKDIR /app

ENTRYPOINT ["dotnet", "test", "-c", "Release"]
