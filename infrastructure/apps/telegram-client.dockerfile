FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY code/apps/TelegramClientApp /app/code/apps/TelegramClientApp
COPY code/libs/DarkDeeds.Authentication /app/code/libs/DarkDeeds.Authentication
COPY code/libs/DarkDeeds.Common /app/code/libs/DarkDeeds.Common
COPY code/libs/DarkDeeds.Communication /app/code/libs/DarkDeeds.Communication
COPY code/apps/TaskServiceApp/DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.Contract /app/code/apps/TaskServiceApp/DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.Contract

WORKDIR /app/code/apps/TelegramClientApp/DarkDeeds.TelegramClientApp/DarkDeeds.TelegramClientApp.App

RUN dotnet build

ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]