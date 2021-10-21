FROM mcr.microsoft.com/dotnet/sdk:5.0

RUN apt-get update && apt-get install -y \
        apt-transport-https \
        ca-certificates \
        curl \
        gnupg \
        hicolor-icon-theme \
        libcanberra-gtk* \
        libgl1-mesa-dri \
        libgl1-mesa-glx \
        libpango1.0-0 \
        libpulse0 \
        libv4l-0 \
        fonts-symbola \
        --no-install-recommends
RUN curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && sh -c 'echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list' \
    && apt-get update \
    && apt-get install google-chrome-stable -y --no-install-recommends \
    && apt-get purge --auto-remove -y curl \
    && rm -rf /var/lib/apt/lists/*

# && apt-get install google-chrome-stable=94.0.4606.61-1 -y --no-install-recommends \

RUN dpkg-reconfigure -f noninteractive tzdata

COPY code/tests/DarkDeeds.E2eTests/ /app
WORKDIR /app

ENTRYPOINT ["dotnet", "test", "--results-directory", "artifacts", "--logger:trx;LogFileName=results.trx"]