echo "Enter migration name (N000_Name): " 
read name
dotnet ef migrations add $name -p ../DarkDeeds/DarkDeeds.Data/DarkDeeds.Data.csproj -s ../DarkDeeds/DarkDeeds.Api/DarkDeeds.Api.csproj