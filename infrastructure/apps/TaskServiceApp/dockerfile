FROM mcr.microsoft.com/dotnet/sdk:5.0-focal

COPY code/apps/TaskServiceApp /app/code/apps/TaskServiceApp
COPY code/libs/DarkDeeds.Authentication /app/code/libs/DarkDeeds.Authentication

WORKDIR /app/code/apps/TaskServiceApp/DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.App

RUN dotnet build

ENV ASPNETCORE_URLS=http://0.0.0.0:80
ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]