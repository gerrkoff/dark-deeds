using Dapper;
using DD.ServiceAuth.Domain.Entities;
using DD.ServiceTask.Domain.Entities;
using MongoDB.Driver;
using Npgsql;

namespace Pg2MongoMigration;

internal static class Pg2Mongo
{
    private static readonly string PgConnectionString = Environment.GetEnvironmentVariable("PG_CONNECTION_STRING") ?? "";

    private static readonly string MongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "";

    public static async Task Run()
    {
        var loadedUsers = await LoadUsers();
        var userNameToIdMapLoaded = UserNameToIdMap(loadedUsers);
        var loadedUsersMongo = loadedUsers.Select(x => x.ToUserEntity()).ToList();
        var mongoDb = GetMongoDatabase();
        await SaveUsers(mongoDb, loadedUsersMongo);
        await UpdateTasks(mongoDb, userNameToIdMapLoaded);

        Console.WriteLine($"Found {loadedUsers.Count} users migrated");
    }

    private static async Task UpdateTasks(IMongoDatabase db, Dictionary<string, string> userNameToIdMap)
    {
        var users = db.GetCollection<UserEntity>("users");
        var tasks = db.GetCollection<TaskEntity>("tasks");
        var plannedRecurrences = db.GetCollection<PlannedRecurrenceEntity>("plannedRecurrences");
        var tasksTotal = 0;
        var plannedRecurrencesTotal = 0;
        foreach (var (key, value) in userNameToIdMap)
        {
            using var cursor =
                await users.FindAsync(Builders<UserEntity>.Filter.Where(x => x.NormalizedUserName == key));
            var newUserId = (await cursor.SingleAsync()).Id.ToString();

            var filter1 = Builders<TaskEntity>.Filter.Where(x => x.UserId == value);
            var update1 = Builders<TaskEntity>.Update.Set<string>(x => x.UserId, newUserId);
            var results1 = await tasks.UpdateManyAsync(filter1, update1);

            Console.WriteLine($"Updated [{key}]-[{value}] to [{newUserId}] {results1.ModifiedCount} tasks");

            var filter2 = Builders<PlannedRecurrenceEntity>.Filter.Where(x => x.UserId == value);
            var update2 = Builders<PlannedRecurrenceEntity>.Update.Set<string>(x => x.UserId, newUserId);
            var results2 = await plannedRecurrences.UpdateManyAsync(filter2, update2);

            Console.WriteLine(
                $"Updated [{key}]-[{value}] to [{newUserId}] {results2.ModifiedCount} planned recurrences");

            tasksTotal += (int)results1.ModifiedCount;
            plannedRecurrencesTotal += (int)results2.ModifiedCount;
        }

        Console.WriteLine("----------------------");
        Console.WriteLine($"Total tasks updated: {tasksTotal}");
        Console.WriteLine($"Total planned recurrences updated: {plannedRecurrencesTotal}");
        Console.WriteLine("----------------------");
    }

    private static async Task<List<UserEntityPg>> LoadUsers()
    {
        await using var connection = new NpgsqlConnection(PgConnectionString);
        await connection.OpenAsync();
        var users = await connection.QueryAsync<UserEntityPg>("SELECT * FROM \"AspNetUsers\"");

        return users.ToList();
    }

    private static Dictionary<string, string> UserNameToIdMap(List<UserEntityPg> users)
    {
        return users.ToDictionary(x => x.NormalizedUserName!, x => x.Id);
    }

    private static IMongoDatabase GetMongoDatabase()
    {
        var client = new MongoClient(MongoConnectionString);
        var dbString = MongoConnectionString.Split('/').Last().Split('?').First();
        return client.GetDatabase(dbString)!;
    }

    private static async Task SaveUsers(IMongoDatabase db, List<UserEntity> users)
    {
        var usersCollection = db.GetCollection<UserEntity>("users");
        var usersTotal = 0;
        foreach (var user in users)
        {
            using var cursor =
                await usersCollection.FindAsync(
                    Builders<UserEntity>.Filter.Where(x => x.NormalizedUserName == user.NormalizedUserName));
            if (await cursor.AnyAsync())
                continue;

            await usersCollection.InsertOneAsync(user);
        }

        Console.WriteLine("----------------------");
        Console.WriteLine($"Total users inserted: {usersTotal}");
        Console.WriteLine("----------------------");
    }
}

internal class UserEntityPg : UserEntity
{
    public new string Id { get; set; } = "";

    public UserEntity ToUserEntity()
    {
        return new UserEntity
        {
            UserName = UserName,
            NormalizedUserName = NormalizedUserName,
            Email = Email,
            NormalizedEmail = NormalizedEmail,
            EmailConfirmed = EmailConfirmed,
            PasswordHash = PasswordHash,
            SecurityStamp = SecurityStamp,
            ConcurrencyStamp = ConcurrencyStamp,
            PhoneNumber = PhoneNumber,
            PhoneNumberConfirmed = PhoneNumberConfirmed,
            TwoFactorEnabled = TwoFactorEnabled,
            LockoutEnd = LockoutEnd,
            LockoutEnabled = LockoutEnabled,
            AccessFailedCount = AccessFailedCount,
            DisplayName = DisplayName
        };
    }
}
