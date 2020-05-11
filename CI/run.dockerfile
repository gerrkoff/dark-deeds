# TODO: try to use 'aspnet' with setting up certificate
FROM mcr.microsoft.com/dotnet/core/sdk:3.1

COPY /src /app
WORKDIR /app

ENV ASPNETCORE_URLS=https://+:80
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "DarkDeeds.Api.dll"]