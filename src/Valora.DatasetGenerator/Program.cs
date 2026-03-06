using Valora.DatasetGenerator.Abstractions;
using Valora.DatasetGenerator.Generators;
using Valora.DatasetGenerator.Models;
using Valora.DatasetGenerator.Registry;
using Valora.DatasetGenerator.Services;

namespace Valora.DatasetGenerator;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            DatasetCategoryGeneratorRegistry registry = BuildRegistry();
            CsvDatasetWriter writer = new();

            if (args.Length == 0)
            {
                PrintUsage(registry);
                return 1;
            }

            if (args.Length == 1 && string.Equals(args[0], "list", StringComparison.OrdinalIgnoreCase))
            {
                PrintCategories(registry);
                return 0;
            }

            string categoryKey = args[0];

            if (!registry.TryGet(categoryKey, out IDatasetCategoryGenerator? generator) || generator is null)
            {
                Console.WriteLine($"Unknown category '{categoryKey}'.");
                Console.WriteLine();
                PrintCategories(registry);
                return 1;
            }

            int count = args.Length >= 2 && int.TryParse(args[1], out int parsedCount)
                ? parsedCount
                : 5000;

            int seed = args.Length >= 3 && int.TryParse(args[2], out int parsedSeed)
                ? parsedSeed
                : 42;

            string repoRoot = ResolveRepoRoot();
            string outputPath = Path.Combine(repoRoot, "data", "datasets", generator.FileName);

            Random random = new(seed);
            IReadOnlyList<object> rows = generator.GenerateRows(count, random);

            writer.Write(outputPath, rows);

            DatasetGenerationResult result = new()
            {
                CategoryKey = generator.Key,
                OutputPath = outputPath,
                RowCount = rows.Count
            };

            PrintResult(result);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Dataset generation failed.");
            Console.WriteLine(ex.Message);
            return 1;
        }
    }

    private static DatasetCategoryGeneratorRegistry BuildRegistry()
    {
        IDatasetCategoryGenerator[] generators =
        [
            new LaptopDatasetCategoryGenerator(),
            new PhoneDatasetCategoryGenerator(),
            new TabletDatasetCategoryGenerator()
        ];

        return new DatasetCategoryGeneratorRegistry(generators);
    }

    private static string ResolveRepoRoot()
    {
        string current = AppContext.BaseDirectory;
        DirectoryInfo? directory = new(current);

        while (directory is not null)
        {
            bool hasSrc = Directory.Exists(Path.Combine(directory.FullName, "src"));
            bool hasData = Directory.Exists(Path.Combine(directory.FullName, "data"));

            if (hasSrc && hasData)
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not resolve repository root.");
    }

    private static void PrintUsage(DatasetCategoryGeneratorRegistry registry)
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  Valora.DatasetGenerator <category> [count] [seed]");
        Console.WriteLine("  Valora.DatasetGenerator list");
        Console.WriteLine();

        PrintCategories(registry);
    }

    private static void PrintCategories(DatasetCategoryGeneratorRegistry registry)
    {
        Console.WriteLine("Available categories:");

        foreach (IDatasetCategoryGenerator generator in registry.GetAll().OrderBy(x => x.Key))
        {
            Console.WriteLine($"  - {generator.Key}");
        }
    }

    private static void PrintResult(DatasetGenerationResult result)
    {
        Console.WriteLine("Dataset generated successfully.");
        Console.WriteLine($"Category : {result.CategoryKey}");
        Console.WriteLine($"Rows     : {result.RowCount}");
        Console.WriteLine($"Output   : {result.OutputPath}");
    }
}