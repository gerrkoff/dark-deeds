echo "Enter start migration name: " 
read name
echo
dotnet ef migrations script $name \
    -p code/backend/DarkDeeds.Backend.Data/DarkDeeds.Backend.Data.csproj
