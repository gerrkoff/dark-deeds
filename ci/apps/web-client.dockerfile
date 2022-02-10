FROM node:16.13.2-alpine as builder

COPY code/ /code/
WORKDIR /code/frontend/app
RUN npm install
RUN npm run test-ci
RUN npm run build

FROM node:16.13.2-alpine

COPY --from=builder /code/frontend/server/ /app/
WORKDIR /app
RUN npm install

ENTRYPOINT ["npm", "run", "start"]
