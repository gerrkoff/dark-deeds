FROM node:12.13.0-alpine as builder

COPY code/ /code/
WORKDIR /code/apps/WebClientApp/app
RUN npm install
RUN npm run test-ci
RUN npm run build
RUN ls

FROM node:12.13.0-alpine

COPY --from=builder /code/apps/WebClientApp/server/ /app/
WORKDIR /app
RUN npm install

ENTRYPOINT ["npm", "run", "start"]
