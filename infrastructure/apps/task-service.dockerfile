FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY code/apps/TaskServiceApp /app/code/apps/TaskServiceApp
COPY code/libs/DarkDeeds.Authentication /app/code/libs/DarkDeeds.Authentication
COPY code/libs/DarkDeeds.Common /app/code/libs/DarkDeeds.Common
COPY code/libs/DarkDeeds.Communication /app/code/libs/DarkDeeds.Communication

WORKDIR /app/code/apps/TaskServiceApp/DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.App

RUN dotnet build

ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]