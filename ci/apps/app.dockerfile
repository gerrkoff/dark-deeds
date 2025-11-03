FROM node:21.4-alpine as builder-fe

WORKDIR /code/frontend

COPY code/frontend/package.json /code/frontend/package.json
COPY code/frontend/package-lock.json /code/frontend/package-lock.json

RUN npm ci --prefer-offline --no-audit

COPY code/frontend/ /code/frontend/
COPY .editorconfig /code/frontend/.editorconfig

RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0.100 AS builder-be

WORKDIR /code/backend

COPY code/backend/Directory.Build.props /code/backend/Directory.Build.props
COPY code/backend/Directory.Packages.props /code/backend/Directory.Packages.props
COPY code/backend/*/*.csproj /code/backend/
RUN for file in $(ls /code/backend/*.csproj); do mkdir -p /code/backend/$(basename $file .csproj); mv $file /code/backend/$(basename $file .csproj); done
COPY code/backend/DarkDeeds.sln /code/backend/DarkDeeds.sln

RUN dotnet restore /code/backend/DarkDeeds.sln

COPY code/backend/ /code/backend/
COPY .editorconfig /code/backend/.editorconfig

RUN dotnet build --no-restore -c Release /code/backend/DarkDeeds.sln

ARG BUILD_VERSION

RUN dotnet publish --no-restore --no-build -c Release -o /build --version-suffix ${BUILD_VERSION} /code/backend/DD.App/DD.App.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=builder-be /build /app/
COPY --from=builder-fe /code/frontend/dist /app/wwwroot/

ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "DD.App.dll"]
