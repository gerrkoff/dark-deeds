echo "Enter migration name (N000_Name): " 
read name
dotnet ef migrations add $name \
    -p code/backend/DarkDeeds.TaskServiceApp.Data/DarkDeeds.TaskServiceApp.Data.csproj  \
    -s code/backend/DarkDeeds.TaskServiceApp.App/DarkDeeds.TaskServiceApp.App.csproj \
