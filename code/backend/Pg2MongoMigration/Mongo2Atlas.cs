using DD.ServiceAuth.Domain.Entities;
using DD.ServiceTask.Domain.Entities;
using DD.TelegramClient.Domain.Entities;
using DD.WebClientBff.Domain.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Pg2MongoMigration;

internal static class Mongo2Atlas
{
    private static readonly string MongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? throw new ArgumentNullException();
    private static readonly string AtlasConnectionString = Environment.GetEnvironmentVariable("ATLAS_CONNECTION_STRING") ?? throw new ArgumentNullException();
    private static readonly bool DryRun = false;

    public static async Task Run()
    {
        RegisterMaps();

        var mongoDb = GetMongoDatabase(MongoConnectionString);
        var atlasDb = GetMongoDatabase(AtlasConnectionString);

        await SaveUsers(atlasDb, await LoadUsers(mongoDb));
        await SaveUserSettings(atlasDb, await LoadUserSettings(mongoDb));
        await SavePlannedRecurrences(atlasDb, await LoadPlannedRecurrences(mongoDb));
        await SaveTelegramUsers(atlasDb, await LoadTelegramUsers(mongoDb));
        await LoadSaveTasks(mongoDb, atlasDb);
    }

    private static async Task<List<UserEntity>> LoadUsers(IMongoDatabase db)
    {
        var usersCollection = db.GetCollection<UserEntity>("users");
        var users = await usersCollection.Find(Builders<UserEntity>.Filter.Empty).ToListAsync();

        return users;
    }

    private static async Task<List<UserSettingsEntity>> LoadUserSettings(IMongoDatabase db)
    {
        var userSettingsCollection = db.GetCollection<UserSettingsEntity>("userSettings");
        var userSettings = await userSettingsCollection.Find(Builders<UserSettingsEntity>.Filter.Empty).ToListAsync();

        return userSettings;
    }

    private static async Task LoadSaveTasks(IMongoDatabase mongoDb, IMongoDatabase atlasDb)
    {
        var mongoTasksCollection = mongoDb.GetCollection<TaskEntity>("tasks");
        var atlasTasksCollection = atlasDb.GetCollection<TaskEntity>("tasks");

        var tasksCursor = await mongoTasksCollection.Find(Builders<TaskEntity>.Filter.Empty).ToCursorAsync();

        var tasksTotal = 0;
        var tasksInserted = 0;
        var tasksSkipped = 0;
        while (await tasksCursor.MoveNextAsync())
        {
            var tasks = tasksCursor.Current.ToList();

            Console.WriteLine($"Batch size: {tasks.Count}.");

            foreach (var task in tasks)
            {
                Console.WriteLine($"Processed {tasksTotal} tasks. Skipped {tasksSkipped}. Inserted {tasksInserted}.");

                tasksTotal++;

                if (await (await atlasTasksCollection.FindAsync(Builders<TaskEntity>.Filter.Where(x => x.Uid == task.Uid))).AnyAsync())
                {
                    tasksSkipped++;
                    continue;
                }

                if (DryRun) continue;

                await atlasTasksCollection.InsertOneAsync(task);
                tasksInserted++;
            }
        }

        Console.WriteLine("----------------------");
        Console.WriteLine($"         Total tasks: {tasksTotal}");
        Console.WriteLine($" Total tasks skipped: {tasksSkipped}");
        Console.WriteLine($"Total tasks inserted: {tasksInserted}");
        Console.WriteLine("----------------------");
    }

    // ReSharper disable once UnusedMember.Local
    private static async Task SaveTasks(IMongoDatabase db, List<TaskEntity> tasks)
    {
        var tasksCollection = db.GetCollection<TaskEntity>("tasks");

        var tasksTotal = 0;
        var tasksSkipped = 0;
        foreach (var task in tasks)
        {
            if (await (await tasksCollection.FindAsync(Builders<TaskEntity>.Filter.Where(x => x.Uid == task.Uid))).AnyAsync())
            {
                tasksSkipped++;
                continue;
            }

            if (DryRun) continue;

            await tasksCollection.InsertOneAsync(task);
            tasksTotal++;
        }

        Console.WriteLine("----------------------");
        Console.WriteLine($"         Total tasks: {tasks.Count}");
        Console.WriteLine($" Total tasks skipped: {tasksSkipped}");
        Console.WriteLine($"Total tasks inserted: {tasksTotal}");
        Console.WriteLine("----------------------");
    }

    private static async Task<List<PlannedRecurrenceEntity>> LoadPlannedRecurrences(IMongoDatabase db)
    {
        var plannedRecurrencesCollection = db.GetCollection<PlannedRecurrenceEntity>("plannedRecurrences");
        var plannedRecurrences = await plannedRecurrencesCollection.Find(Builders<PlannedRecurrenceEntity>.Filter.Empty).ToListAsync();

        return plannedRecurrences;
    }

    private static async Task<List<TelegramUserEntity>> LoadTelegramUsers(IMongoDatabase db)
    {
        var telegramUsersCollection = db.GetCollection<TelegramUserEntity>("telegramUsers");
        var telegramUsers = await telegramUsersCollection.Find(Builders<TelegramUserEntity>.Filter.Empty).ToListAsync();

        return telegramUsers;
    }

    private static async Task SaveTelegramUsers(IMongoDatabase db, List<TelegramUserEntity> telegramUsers)
    {
        var telegramUsersCollection = db.GetCollection<TelegramUserEntity>("telegramUsers");

        var telegramUsersTotal = 0;
        var telegramUsersSkipped = 0;
        foreach (var telegramUser in telegramUsers)
        {
            if (await (await telegramUsersCollection.FindAsync(Builders<TelegramUserEntity>.Filter.Where(x => x.UserId == telegramUser.UserId))).AnyAsync())
            {
                telegramUsersSkipped++;
                continue;
            }

            if (DryRun) continue;

            await telegramUsersCollection.InsertOneAsync(telegramUser);
            telegramUsersTotal++;
        }

        Console.WriteLine("----------------------");
        Console.WriteLine($"         Total telegram users: {telegramUsers.Count}");
        Console.WriteLine($" Total telegram users skipped: {telegramUsersSkipped}");
        Console.WriteLine($"Total telegram users inserted: {telegramUsersTotal}");
        Console.WriteLine("----------------------");
    }

    private static async Task SavePlannedRecurrences(IMongoDatabase db, List<PlannedRecurrenceEntity> plannedRecurrences)
    {
        var plannedRecurrencesCollection = db.GetCollection<PlannedRecurrenceEntity>("plannedRecurrences");

        var plannedRecurrencesTotal = 0;
        var plannedRecurrencesSkipped = 0;
        foreach (var plannedRecurrence in plannedRecurrences)
        {
            if (await (await plannedRecurrencesCollection.FindAsync(Builders<PlannedRecurrenceEntity>.Filter.Where(x => x.Uid == plannedRecurrence.Uid))).AnyAsync())
            {
                plannedRecurrencesSkipped++;
                continue;
            }

            if (DryRun) continue;

            await plannedRecurrencesCollection.InsertOneAsync(plannedRecurrence);
            plannedRecurrencesTotal++;
        }

        Console.WriteLine("----------------------");
        Console.WriteLine($"         Total planned recurrences: {plannedRecurrences.Count}");
        Console.WriteLine($" Total planned recurrences skipped: {plannedRecurrencesSkipped}");
        Console.WriteLine($"Total planned recurrences inserted: {plannedRecurrencesTotal}");
        Console.WriteLine("----------------------");
    }

    private static async Task SaveUserSettings(IMongoDatabase db, List<UserSettingsEntity> userSettings)
    {
        var userSettingsCollection = db.GetCollection<UserSettingsEntity>("userSettings");

        var userSettingsTotal = 0;
        var userSettingsSkipped = 0;
        foreach (var userSetting in userSettings)
        {
            if (await (await userSettingsCollection.FindAsync(Builders<UserSettingsEntity>.Filter.Where(x => x.UserId == userSetting.UserId))).AnyAsync())
            {
                userSettingsSkipped++;
                continue;
            }

            if (DryRun) continue;

            await userSettingsCollection.InsertOneAsync(userSetting);
            userSettingsTotal++;
        }

        Console.WriteLine("----------------------");
        Console.WriteLine($"         Total user settings: {userSettings.Count}");
        Console.WriteLine($" Total user settings skipped: {userSettingsSkipped}");
        Console.WriteLine($"Total user settings inserted: {userSettingsTotal}");
        Console.WriteLine("----------------------");
    }

    private static async Task SaveUsers(IMongoDatabase db, List<UserEntity> users)
    {
        var usersCollection = db.GetCollection<UserEntity>("users");

        var usersTotal = 0;
        var usersSkipped = 0;
        foreach (var user in users)
        {
            if (await (await usersCollection.FindAsync(Builders<UserEntity>.Filter.Where(x => x.Id == user.Id))).AnyAsync())
            {
                usersSkipped++;
                continue;
            }

            if (DryRun) continue;

            await usersCollection.InsertOneAsync(user);
            usersTotal++;
        }

        Console.WriteLine("----------------------");
        Console.WriteLine($"         Total users: {users.Count}");
        Console.WriteLine($" Total users skipped: {usersSkipped}");
        Console.WriteLine($"Total users inserted: {usersTotal}");
        Console.WriteLine("----------------------");
    }

    private static IMongoDatabase GetMongoDatabase(string connectionString)
    {
        var client = new MongoClient(connectionString);
        var dbString = connectionString.Split('/').Last().Split('?').First();
        return client.GetDatabase(dbString) ?? throw new ArgumentNullException();
    }

    private static void RegisterMaps()
    {
        RegisterDefaultMap<PlannedRecurrenceEntity>();
        RegisterDefaultMap<RecurrenceEntity>();
        RegisterDefaultMap<TaskEntity>();
        RegisterDefaultMap<UserEntity>();
        RegisterDefaultMap<UserSettingsEntity>();
        RegisterDefaultMap<TelegramUserEntity>();
    }

    private static void RegisterDefaultMap<TEntity>()
    {
        BsonClassMap.RegisterClassMap<TEntity>(map =>
        {
            map.AutoMap();
            map.SetIgnoreExtraElements(true);
        });
    }
}
