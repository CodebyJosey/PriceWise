using PriceWise.DatasetGenerator.Generators;

const int defaultCount = 1000;
const int defaultSeed = 42;

string repoRoot = Directory.GetParent(AppContext.BaseDirectory)!
    .Parent!
    .Parent!
    .Parent!
    .Parent!
    .FullName;

string outputPath = Path.Combine(repoRoot, "..", "data", "datasets", "laptops.csv");

int count = defaultCount;
int seed = defaultSeed;

if (args.Length >= 1 && int.TryParse(args[0], out int parsedCount) && parsedCount > 0)
{
    count = parsedCount;
}

if (args.Length >= 2 && int.TryParse(args[1], out int parsedSeed))
{
    seed = parsedSeed;
}

LaptopDatasetGenerator? generator = new LaptopDatasetGenerator();
IReadOnlyList<PriceWise.DatasetGenerator.Models.LaptopDatasetRow> rows = generator.Generate(count, seed);
generator.WriteCsv(outputPath, rows);

Console.WriteLine($"Generated {rows.Count} laptop rows.");
Console.WriteLine($"Output: {outputPath}");
Console.WriteLine($"Seed: {seed}");