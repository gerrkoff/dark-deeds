FROM node:12.13.0-alpine

COPY code/apps/WebClientApp/server/package.json /app/server_deps/
WORKDIR /app/server_deps
RUN npm install

COPY code/apps/WebClientApp/app/package.json /app/app_deps/
WORKDIR /app/app_deps
RUN npm install

COPY code/apps/WebClientApp /app/code/apps/WebClientApp

WORKDIR /app/code/apps/WebClientApp/app
RUN mv /app/app_deps/node_modules /app/code/apps/WebClientApp/app/node_modules
RUN npm run build

WORKDIR /app/code/apps/WebClientApp/server
RUN mv /app/server_deps/node_modules /app/code/apps/WebClientApp/server/node_modules

ENTRYPOINT ["npm", "run", "start"]