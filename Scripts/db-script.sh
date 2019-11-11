echo "Enter start migration name: " 
read name
echo
dotnet ef migrations script $name -p ../DarkDeeds/DarkDeeds.Data/DarkDeeds.Data.csproj -s ../DarkDeeds/DarkDeeds.Api/DarkDeeds.Api.csproj