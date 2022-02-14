FROM node:16.13.2-alpine as builder

COPY code/frontend/ /code/frontend/
COPY .editorconfig /code/frontend/.editorconfig
WORKDIR /code/frontend/app
RUN npm install
RUN npm run build
RUN npm run test-ci

FROM node:16.13.2-alpine

COPY --from=builder /code/frontend/server/ /app/
WORKDIR /app
RUN npm install

ENTRYPOINT ["npm", "run", "start"]
