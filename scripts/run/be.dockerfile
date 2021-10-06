FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN dotnet tool install --global dotnet-ef

COPY DarkDeeds /app
COPY Scripts/run/be.sh /app/
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:80

ENTRYPOINT ["/bin/sh", "-c", "./be.sh"]