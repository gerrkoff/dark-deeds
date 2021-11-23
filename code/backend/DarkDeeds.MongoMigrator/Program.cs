using System;
using System.Linq;
using System.Threading.Tasks;
using DarkDeeds.MongoMigrator.Helpers;
using DarkDeeds.MongoMigrator.Processors;
using DarkDeeds.MongoMigrator.RepoFactory;
using Microsoft.Extensions.Configuration;

namespace DarkDeeds.MongoMigrator
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var dryRun = args.Length == 0 || args[0] != "run";
            Console.WriteLine($"Mongo Migrator, dry-run={dryRun}");
            Console.WriteLine();

            var config = new ConfigFactory().Get();
            Console.WriteLine($"from: {config.GetConnectionString("pgDb")}");
            Console.WriteLine($"to: {config.GetConnectionString("mongoDb")}");
            Console.WriteLine();
            
            var (pgTaskRepo, pgPlannedRecurrenceRepo, pgRecurrenceRepo) = new RepoFactoryPg().CreateRepos(config);
            var (mongoTaskRepo, mongoPlannedRecurrenceRepo) = new RepoFactoryMongo().CreateRepos(config);
            var scaleRenderer = new ScaleRenderer();

            var totalTasks = pgTaskRepo.GetAll().Count();
            var totalPlannedRecurrences = pgPlannedRecurrenceRepo.GetAll().Count();
            var totalRecurrences = pgRecurrenceRepo.GetAll().Count();
            
            Console.WriteLine($"{totalTasks}\ttasks found");
            Console.WriteLine($"{totalPlannedRecurrences}\tplanned recurrences found");
            Console.WriteLine($"{totalRecurrences}\trecurrences found");
            Console.WriteLine();

            Console.WriteLine("syncing tasks...");
            scaleRenderer.Prepare(totalTasks);
            var (tasksCreated, tasksUpdated, tasksWarning) = await new ProcessorTasks(pgTaskRepo, mongoTaskRepo, dryRun, scaleRenderer.RenderProgress).Process();
            Console.WriteLine($"{tasksCreated}\ttasks created");
            Console.WriteLine($"{tasksUpdated}\ttasks updated");
            Console.WriteLine($"{tasksWarning}\ttasks warning");
            Console.WriteLine($"{tasksCreated + tasksUpdated}/{totalTasks}");
            Console.WriteLine();
            
            Console.WriteLine("syncing planned recurrences...");
            scaleRenderer.Prepare(totalPlannedRecurrences);
            var (plannedRecurrencesCreated, plannedRecurrencesUpdated, plannedRecurrencesWarning) = await new ProcessorPlannedRecurrences(pgPlannedRecurrenceRepo, mongoPlannedRecurrenceRepo, dryRun, scaleRenderer.RenderProgress).Process();
            Console.WriteLine($"{plannedRecurrencesCreated}\tplanned recurrences created");
            Console.WriteLine($"{plannedRecurrencesUpdated}\tplanned recurrences updated");
            Console.WriteLine($"{plannedRecurrencesWarning}\tplanned recurrences warning");
            Console.WriteLine($"{plannedRecurrencesCreated + plannedRecurrencesUpdated}/{totalPlannedRecurrences}");
            Console.WriteLine();
            
            Console.WriteLine("syncing recurrences...");
            scaleRenderer.Prepare(totalRecurrences);
            var (recurrencesCreated, recurrencesSkipped, recurrencesWarning) = await new ProcessorRecurrences(pgRecurrenceRepo, mongoPlannedRecurrenceRepo, dryRun, scaleRenderer.RenderProgress).Process();
            Console.WriteLine($"{recurrencesCreated}\trecurrences created");
            Console.WriteLine($"{recurrencesSkipped}\trecurrences skipped");
            Console.WriteLine($"{recurrencesWarning}\trecurrences warning");
            Console.WriteLine($"{recurrencesCreated + recurrencesSkipped}/{totalRecurrences}");
            Console.WriteLine();
        }
    }
}