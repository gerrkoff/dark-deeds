FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

COPY code/backend/ /code/backend/
COPY .editorconfig /code/backend/.editorconfig
WORKDIR /code
ARG BUILD_VERSION

RUN dotnet publish -c Release -o /build --version-suffix ${BUILD_VERSION}  \
    /code/backend/DarkDeeds.ServiceAuth.App/DarkDeeds.ServiceAuth.App.csproj

FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY --from=builder /build /app/
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DarkDeeds.ServiceAuth.App.dll"]
