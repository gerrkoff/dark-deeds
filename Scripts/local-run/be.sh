cd DarkDeeds.Api/
dotnet build
dotnet ef database update -p ../DarkDeeds.Data/DarkDeeds.Data.csproj -s DarkDeeds.Api.csproj
dotnet bin/Debug/netcoreapp2.2/DarkDeeds.Api.dll