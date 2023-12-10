FROM mcr.microsoft.com/dotnet/sdk:8.0

COPY code/backend /app/

WORKDIR /app/DarkDeeds.Backend.App

RUN dotnet build

ENV ASPNETCORE_ENVIRONMENT=Staging

ENTRYPOINT ["dotnet", "run", "--no-launch-profile"]
