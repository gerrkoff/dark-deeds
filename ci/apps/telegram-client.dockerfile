FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

COPY code/ /code/
WORKDIR /code
ARG BUILD_VERSION

RUN dotnet test "--logger:trx;LogFileName=results.trx" --results-directory /test-results \
    /code/apps/TelegramClientApp/DarkDeeds.TelegramClientApp/DarkDeeds.TelegramClientApp.Tests/DarkDeeds.TelegramClientApp.Tests.csproj
RUN dotnet publish -c Release -o /build --version-suffix ${BUILD_VERSION}  \
    /code/apps/TelegramClientApp/DarkDeeds.TelegramClientApp/DarkDeeds.TelegramClientApp.App/DarkDeeds.TelegramClientApp.App.csproj

FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY --from=builder /build /app/
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DarkDeeds.TelegramClientApp.App.dll"]
