echo "UPDATING VERSION"
echo "Enter version (PATCH): " 
read version

dotnet dotnet-bumpversion/BumpVersion.dll /Users/anton.prokofiev/dark-deeds/DarkDeeds/DarkDeeds.Api/DarkDeeds.Api.csproj $version