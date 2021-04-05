FROM mcr.microsoft.com/dotnet/sdk:5.0-focal

COPY code/apps/AuthServiceApp /app/code/apps/AuthServiceApp
COPY code/libs/DarkDeeds.Authentication /app/code/libs/DarkDeeds.Authentication

WORKDIR /app/code/apps/AuthServiceApp/DarkDeeds.AuthServiceApp/DarkDeeds.AuthServiceApp.App

RUN dotnet build

ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]