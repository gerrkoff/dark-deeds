FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

COPY /src /app
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "DarkDeeds.Api.dll"]