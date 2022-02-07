echo "Enter migration name: " 
read name
dotnet ef migrations add $name \
    -p code/backend/DarkDeeds.Backend.Data/DarkDeeds.Backend.Data.csproj
