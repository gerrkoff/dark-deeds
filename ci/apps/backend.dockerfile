FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

COPY code/ /code/
WORKDIR /code
ARG BUILD_VERSION

RUN dotnet test "--logger:trx;LogFileName=results.trx" --results-directory /test-results \
    /code/backend/DarkDeeds.TelegramClient.Tests/DarkDeeds.TelegramClient.Tests.csproj
RUN dotnet publish -c Release -o /build --version-suffix ${BUILD_VERSION}  \
    /code/backend/DarkDeeds.Backend.App/DarkDeeds.Backend.App.csproj

FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY --from=builder /build /app/
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DarkDeeds.Backend.App.dll"]
