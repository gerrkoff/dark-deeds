echo "Enter migration name (N000_Name): " 
read name
dotnet ef migrations add $name \
    -p DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.Data/DarkDeeds.TaskServiceApp.Data.csproj  \
    -s DarkDeeds.TaskServiceApp/DarkDeeds.TaskServiceApp.App/DarkDeeds.TaskServiceApp.App.csproj \
