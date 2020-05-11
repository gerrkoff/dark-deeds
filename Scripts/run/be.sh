cd DarkDeeds.Api/
dotnet build
/root/.dotnet/tools/dotnet-ef database update -p ../DarkDeeds.Data/DarkDeeds.Data.csproj -s DarkDeeds.Api.csproj
dotnet bin/Debug/netcoreapp3.1/DarkDeeds.Api.dll