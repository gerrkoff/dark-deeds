FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

COPY code/ /code/
WORKDIR /code
ARG BUILD_VERSION

RUN dotnet publish -c Release -o /build --version-suffix ${BUILD_VERSION}  \
    /code/apps/ApiGatewayApp/DarkDeeds.ApiGatewayApp/DarkDeeds.ApiGatewayApp.App/DarkDeeds.ApiGatewayApp.App.csproj

FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY --from=builder /build /app/
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DarkDeeds.ApiGatewayApp.App.dll"]
