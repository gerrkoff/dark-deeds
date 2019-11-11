FROM mcr.microsoft.com/dotnet/core/sdk:2.2

COPY DarkDeeds /app
COPY Scripts/run-local/run-be.sh /app/
WORKDIR /app

ENTRYPOINT ["/bin/sh", "-c", "./be.sh"]