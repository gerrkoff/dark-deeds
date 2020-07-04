FROM mcr.microsoft.com/dotnet/core/sdk:2.2

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
        --no-install-recommends \
    && curl -sSL https://dl.google.com/linux/linux_signing_key.pub | apt-key add - \
    && echo "deb [arch=amd64] https://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list \
    && apt-get update && apt-get install -y \
        chromium-browser=81.0.4044.138-0ubuntu0.16.04.1 \
        --no-install-recommends \
    && apt-get purge --auto-remove -y curl \
    && rm -rf /var/lib/apt/lists/*

RUN dpkg-reconfigure -f noninteractive tzdata

COPY DarkDeeds.E2eTests/ /app
COPY CI/test-e2e.sh /app
WORKDIR /app

ENTRYPOINT ["/bin/sh", "-c", "./test-e2e.sh"]
