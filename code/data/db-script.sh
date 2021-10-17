echo "Enter start migration name: " 
read name
echo
dotnet ef migrations script $name \
    -p code/backend/DarkDeeds.TaskServiceApp.Data/DarkDeeds.TaskServiceApp.Data.csproj  \
    -s code/backend/DarkDeeds.TaskServiceApp.App/DarkDeeds.TaskServiceApp.App.csproj \
    > init.sql