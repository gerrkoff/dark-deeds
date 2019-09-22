echo
echo "      ─▄▀─▄▀"
echo "      ──▀──▀"
echo "      █▀▀▀▀▀█▄     --------------------------------"
echo "      █░░░░░█─█    | It's time to update version! |"
echo "      ▀▄▄▄▄▄▀▀     --------------------------------"
echo
echo "Enter version (PATCH): " 
read version

dotnet dotnet-bumpversion/BumpVersion.dll ../DarkDeeds/DarkDeeds.Api/DarkDeeds.Api.csproj $version
