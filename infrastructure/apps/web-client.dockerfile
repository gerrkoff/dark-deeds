FROM node:16.13.2-alpine

COPY code/frontend/server/package.json /app/server_deps/
COPY code/frontend/server/package-lock.json /app/server_deps/
WORKDIR /app/server_deps
RUN npm install

COPY code/frontend/app/package.json /app/app_deps/
COPY code/frontend/app/package-lock.json /app/app_deps/
WORKDIR /app/app_deps
RUN npm install

COPY code/frontend /app/

WORKDIR /app/app
RUN mv /app/app_deps/node_modules /app/app/node_modules
RUN npm run build

WORKDIR /app/server
RUN mv /app/server_deps/node_modules /app/server/node_modules

ENTRYPOINT ["npm", "run", "start"]
