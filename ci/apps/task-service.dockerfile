FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder

COPY code/backend/ /code/backend/
COPY .editorconfig /code/backend/.editorconfig
WORKDIR /code
ARG BUILD_VERSION

RUN dotnet test "--logger:trx;LogFileName=results.trx" --results-directory /test-results \
    /code/backend/DarkDeeds.ServiceTask.Tests/DarkDeeds.ServiceTask.Tests.csproj
RUN dotnet publish -c Release -o /build --version-suffix ${BUILD_VERSION}  \
    /code/backend/DarkDeeds.ServiceTask.App/DarkDeeds.ServiceTask.App.csproj

FROM mcr.microsoft.com/dotnet/aspnet:5.0

COPY --from=builder /build /app/
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "DarkDeeds.ServiceTask.App.dll"]
