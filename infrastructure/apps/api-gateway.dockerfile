FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY code/apps/ApiGatewayApp /app/code/apps/ApiGatewayApp

WORKDIR /app/code/apps/ApiGatewayApp/DarkDeeds.ApiGatewayApp/DarkDeeds.ApiGatewayApp.App

RUN dotnet build

ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]