FROM node:21.4-alpine as builder-fe

WORKDIR /code/frontend

COPY code/frontend/package.json /code/frontend/package.json
COPY code/frontend/package-lock.json /code/frontend/package-lock.json

RUN npm install

COPY code/frontend/ /code/frontend/
COPY .editorconfig /code/frontend/.editorconfig

RUN npm run build
RUN npm run test-ci

FROM node:21.4-alpine as builder-fe-4

WORKDIR /code/frontend4

COPY code/frontend4/package.json /code/frontend4/package.json
COPY code/frontend4/package-lock.json /code/frontend4/package-lock.json

RUN npm install

COPY code/frontend4/ /code/frontend4/
COPY .editorconfig /code/frontend4/.editorconfig

RUN npm run ci

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

RUN dotnet build --no-restore -warnaserror /code/backend/DarkDeeds.sln
RUN dotnet test --no-restore "--logger:trx;LogFileName=results.trx" --results-directory /test-results /code/backend/DarkDeeds.sln

ARG BUILD_VERSION

RUN dotnet publish -c Release -o /build --version-suffix ${BUILD_VERSION} /code/backend/DD.App/DD.App.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=builder-be /build /app/
COPY --from=builder-fe /code/frontend/build /app/wwwroot/old
COPY --from=builder-fe-4 /code/frontend4/dist /app/wwwroot/

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DD.App.dll"]
