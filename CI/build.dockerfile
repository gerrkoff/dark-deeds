FROM mcr.microsoft.com/dotnet/core/sdk:3.1

RUN curl -sL https://deb.nodesource.com/setup_12.x | bash -
RUN apt-get install -y nodejs
RUN npm install -g npm

COPY DarkDeeds /app/DarkDeeds
COPY DarkDeeds.WebClient /app/DarkDeeds.WebClient
COPY CI /app/CI
COPY Tools /app/Tools
WORKDIR /app

ENTRYPOINT ["/bin/sh", "-c", "cd CI && ./build.sh"]