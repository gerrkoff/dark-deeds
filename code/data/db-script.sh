echo "Enter start migration name: "
read name
echo
dotnet ef migrations script $name \
    -p code/backend/DD.Shared.Data/DD.Shared.Data.csproj
