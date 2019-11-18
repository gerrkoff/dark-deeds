FROM node:12.13.0-alpine

COPY DarkDeeds.WebClient /app
COPY Scripts/run/fe.sh /app/
WORKDIR /app

ENTRYPOINT ["/bin/sh", "-c", "./fe.sh"]