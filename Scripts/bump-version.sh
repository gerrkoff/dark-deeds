echo "UPDATING VERSION"
echo "Enter version (PATCH): " 
read version

dotnet dotnet-bumpversion/BumpVersion.dll ../DarkDeeds/DarkDeeds.Api/DarkDeeds.Api.csproj $version