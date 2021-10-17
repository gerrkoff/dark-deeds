FROM mcr.microsoft.com/dotnet/sdk:5.0

COPY code/backend /app/

WORKDIR /app/DarkDeeds.WebClientBffApp.App

RUN dotnet build

ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]