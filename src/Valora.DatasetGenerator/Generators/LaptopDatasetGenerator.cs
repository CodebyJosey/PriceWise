using Valora.DatasetGenerator.Abstractions;
using Valora.DatasetGenerator.Rows;

namespace Valora.DatasetGenerator.Generators;

/// <summary>
/// Generates laptop dataset rows.
/// </summary>
public sealed class LaptopDatasetCategoryGenerator : IDatasetCategoryGenerator
{
    private static readonly string[] Brands =
    [
        "Lenovo", "HP", "Dell", "Asus", "Acer", "MSI", "Apple"
    ];

    private static readonly string[] Cpus =
    [
        "Intel i5", "Intel i7", "Intel i9", "Ryzen 5", "Ryzen 7", "Ryzen 9", "Apple M1", "Apple M2", "Apple M3"
    ];

    private static readonly string[] Gpus =
    [
        "Integrated", "GTX 1650", "RTX 3050", "RTX 3060", "RTX 4060", "RTX 4070", "Radeon Graphics"
    ];

    private static readonly string[] Conditions =
    [
        "Used", "Refurbished", "As New", "New"
    ];

    private static readonly string[] Segments =
    [
        "Budget", "Ultrabook", "Business", "Gaming", "Creator", "Premium"
    ];

    /// <inheritdoc />
    public string Key => "laptops";

    /// <inheritdoc />
    public string FileName => "laptops.csv";

    /// <inheritdoc />
    public IReadOnlyList<object> GenerateRows(int count, Random random)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        ArgumentNullException.ThrowIfNull(random);

        List<object> rows = new(count);

        for (int i = 0; i < count; i++)
        {
            string brand = Pick(Brands, random);
            string cpu = Pick(Cpus, random);
            float ramGb = Pick([8f, 16f, 32f, 64f], random);
            float storageGb = Pick([256f, 512f, 1024f, 2048f], random);
            string gpu = Pick(Gpus, random);
            float screenSizeInch = Pick([13.3f, 14f, 15.6f, 16f, 17.3f], random);
            float refreshRate = Pick([60f, 120f, 144f, 165f, 240f], random);
            float releaseYear = random.Next(2019, 2027);
            string condition = Pick(Conditions, random);
            string segment = Pick(Segments, random);

            float price = CalculateLaptopPrice(
                brand,
                cpu,
                ramGb,
                storageGb,
                gpu,
                screenSizeInch,
                refreshRate,
                releaseYear,
                condition,
                segment,
                random);

            rows.Add(new LaptopDatasetRow
            {
                Brand = brand,
                Cpu = cpu,
                RamGb = ramGb,
                StorageGb = storageGb,
                Gpu = gpu,
                ScreenSizeInch = screenSizeInch,
                RefreshRate = refreshRate,
                ReleaseYear = releaseYear,
                Condition = condition,
                Segment = segment,
                Price = price
            });
        }

        return rows;
    }

    private static float CalculateLaptopPrice(
        string brand,
        string cpu,
        float ramGb,
        float storageGb,
        string gpu,
        float screenSizeInch,
        float refreshRate,
        float releaseYear,
        string condition,
        string segment,
        Random random)
    {
        float price = 250f;

        price += brand switch
        {
            "Apple" => 700f,
            "MSI" => 250f,
            "Dell" => 120f,
            "Lenovo" => 100f,
            "Asus" => 90f,
            "HP" => 70f,
            _ => 50f
        };

        price += cpu switch
        {
            "Intel i9" => 450f,
            "Ryzen 9" => 400f,
            "Apple M3" => 500f,
            "Apple M2" => 350f,
            "Apple M1" => 250f,
            "Intel i7" => 220f,
            "Ryzen 7" => 200f,
            "Intel i5" => 120f,
            "Ryzen 5" => 110f,
            _ => 0f
        };

        price += gpu switch
        {
            "RTX 4070" => 700f,
            "RTX 4060" => 500f,
            "RTX 3060" => 350f,
            "RTX 3050" => 220f,
            "GTX 1650" => 120f,
            "Radeon Graphics" => 70f,
            _ => 0f
        };

        price += ramGb * 12f;
        price += storageGb * 0.22f;
        price += (screenSizeInch - 13f) * 18f;
        price += (refreshRate - 60f) * 1.8f;
        price += (releaseYear - 2019f) * 70f;

        price += segment switch
        {
            "Premium" => 350f,
            "Creator" => 280f,
            "Gaming" => 300f,
            "Business" => 140f,
            "Ultrabook" => 180f,
            "Budget" => -40f,
            _ => 0f
        };

        price += condition switch
        {
            "New" => 220f,
            "As New" => 120f,
            "Refurbished" => 40f,
            "Used" => -80f,
            _ => 0f
        };

        price += (float)((random.NextDouble() - 0.5) * 120.0);

        return MathF.Max(150f, MathF.Round(price, 2));
    }

    private static T Pick<T>(IReadOnlyList<T> values, Random random)
        => values[random.Next(values.Count)];
}