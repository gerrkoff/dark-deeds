FROM node:8

COPY DarkDeeds.WebClient /app
COPY Scripts/run-local/run-fe.sh /app/
WORKDIR /app

ENTRYPOINT ["/bin/sh", "-c", "./fe.sh"]