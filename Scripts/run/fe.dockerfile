FROM node:8

COPY DarkDeeds.WebClient /app
COPY Scripts/run/fe.sh /app/
WORKDIR /app

ENTRYPOINT ["/bin/sh", "-c", "./fe.sh"]