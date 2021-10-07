FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY code/apps/WebClientBffApp /app/code/apps/WebClientBffApp
COPY code/libs/DarkDeeds.Authentication /app/code/libs/DarkDeeds.Authentication
COPY code/libs/DarkDeeds.Common /app/code/libs/DarkDeeds.Common
COPY code/libs/DarkDeeds.Communication /app/code/libs/DarkDeeds.Communication
COPY code/apps/AuthServiceApp/DarkDeeds.AuthServiceApp/DarkDeeds.AuthServiceApp.Contract /app/code/apps/AuthServiceApp/DarkDeeds.AuthServiceApp/DarkDeeds.AuthServiceApp.Contract
COPY code/apps/TaskServiceApp/DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.Contract /app/code/apps/TaskServiceApp/DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.Contract

WORKDIR /app/code/apps/WebClientBffApp/DarkDeeds.WebClientBffApp/DarkDeeds.WebClientBffApp.App

RUN dotnet build

ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]