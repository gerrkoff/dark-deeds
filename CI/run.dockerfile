FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

COPY /src /app
WORKDIR /app

# ENV ASPNETCORE_URLS=https://+:80
ENV ASPNETCORE_ENVIRONMENT Production

ENTRYPOINT ["dotnet", "DarkDeeds.Api.dll"]