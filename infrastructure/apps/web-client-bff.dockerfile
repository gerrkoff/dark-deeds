FROM mcr.microsoft.com/dotnet/sdk:5.0-focal

COPY code/apps/WebClientBffApp /app/code/apps/WebClientBffApp
COPY code/libs/DarkDeeds.Authentication /app/code/libs/DarkDeeds.Authentication

WORKDIR /app/code/apps/WebClientBffApp/DarkDeeds.WebClientBffApp/DarkDeeds.WebClientBffApp.App

RUN dotnet build

ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]