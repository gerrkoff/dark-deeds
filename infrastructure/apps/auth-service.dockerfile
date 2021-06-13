FROM mcr.microsoft.com/dotnet/sdk:5.0-focal

COPY code/apps/AuthServiceApp /app/code/apps/AuthServiceApp
COPY code/libs/DarkDeeds.Authentication /app/code/libs/DarkDeeds.Authentication
COPY code/libs/DarkDeeds.Common /app/code/libs/DarkDeeds.Common
COPY code/libs/DarkDeeds.Communication /app/code/libs/DarkDeeds.Communication

WORKDIR /app/code/apps/AuthServiceApp/DarkDeeds.AuthServiceApp/DarkDeeds.AuthServiceApp.App

RUN dotnet build

ENV ASPNETCORE_URLS=https://0.0.0.0:5002
ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]