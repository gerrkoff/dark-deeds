using Pg2MongoMigration;

// await Pg2Mongo.Run();
await Mongo2Atlas.Run();

Console.WriteLine("Migration completed");
