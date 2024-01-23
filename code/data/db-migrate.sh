echo "Enter migration name: "
read name
dotnet ef migrations add $name \
    -p code/backend/DD.Data/DD.Data.csproj
