echo "Enter start migration name: "
read name
echo
dotnet ef migrations script $name \
    -p code/backend/DD.Data/DD.Data.csproj
