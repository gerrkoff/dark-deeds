FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /app

COPY code/mcp/DarkDeeds.Mcp/DarkDeeds.Mcp.sln /app/DarkDeeds.Mcp.sln
COPY code/mcp/DarkDeeds.Mcp/DarkDeeds.Mcp.csproj /app/DarkDeeds.Mcp.csproj

RUN dotnet restore /app/DarkDeeds.Mcp.sln

COPY code/mcp/DarkDeeds.Mcp/ /app
COPY .editorconfig /app/.editorconfig

RUN dotnet build --no-restore -warnaserror /app/DarkDeeds.Mcp.sln

ARG BUILD_VERSION

RUN dotnet publish -c Release -o /build --version-suffix ${BUILD_VERSION} /app/DarkDeeds.Mcp.sln

FROM mcr.microsoft.com/dotnet/runtime:8.0

COPY --from=builder /build /app/
WORKDIR /app

ENTRYPOINT ["dotnet", "DarkDeeds.Mcp.dll"]
