FROM mcr.microsoft.com/dotnet/core/sdk:3.1

COPY DarkDeeds /app
COPY Scripts/run/be.sh /app/
WORKDIR /app

ENTRYPOINT ["/bin/sh", "-c", "./be.sh"]