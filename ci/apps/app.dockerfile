FROM node:21.4-alpine as builder-fe

COPY code/frontend/ /code/frontend/
COPY .editorconfig /code/frontend/.editorconfig
WORKDIR /code/frontend
RUN npm install
RUN npm run build
RUN npm run test-ci

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder-be

COPY code/backend/ /code/backend/
COPY .editorconfig /code/backend/.editorconfig
WORKDIR /code/backend
ARG BUILD_VERSION

RUN dotnet build -warnaserror /code/backend/DarkDeeds.sln
RUN dotnet test "--logger:trx;LogFileName=results.trx" --results-directory /test-results /code/backend/DarkDeeds.sln
RUN dotnet publish -c Release -o /build --version-suffix ${BUILD_VERSION} /code/backend/DD.App/DD.App.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0

COPY --from=builder-be /build /app/
COPY --from=builder-fe /code/frontend/build /app/wwwroot/
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DD.App.dll"]
