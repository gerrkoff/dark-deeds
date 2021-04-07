echo "Enter start migration name: " 
read name
echo
dotnet ef migrations script $name \
    -p DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.Data/DarkDeeds.TaskServiceApp.Data.csproj  \
    -s DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.App/DarkDeeds.TaskServiceApp.App.csproj \
    >> init.sql