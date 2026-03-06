using Valora.DatasetGenerator.Abstractions;
using Valora.DatasetGenerator.Rows;

namespace Valora.DatasetGenerator.Generators;

/// <summary>
/// Generates phone dataset rows.
/// </summary>
public sealed class PhoneDatasetCategoryGenerator : IDatasetCategoryGenerator
{
    private static readonly string[] Brands =
    [
        "Apple", "Samsung", "Google", "OnePlus", "Xiaomi"
    ];

    private static readonly Dictionary<string, string[]> ModelFamilies = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Apple"] = ["iPhone 11", "iPhone 12", "iPhone 13", "iPhone 14", "iPhone 15"],
        ["Samsung"] = ["Galaxy S21", "Galaxy S22", "Galaxy S23", "Galaxy S24", "Galaxy A54"],
        ["Google"] = ["Pixel 6", "Pixel 7", "Pixel 8", "Pixel 8a"],
        ["OnePlus"] = ["OnePlus 10", "OnePlus 11", "OnePlus 12", "Nord 3"],
        ["Xiaomi"] = ["Mi 11", "12T", "13T", "Redmi Note 12"]
    };

    private static readonly string[] Conditions =
    [
        "Used", "Refurbished", "As New", "New"
    ];

    /// <inheritdoc />
    public string Key => "phones";

    /// <inheritdoc />
    public string FileName => "phones.csv";

    /// <inheritdoc />
    public IReadOnlyList<object> GenerateRows(int count, Random random)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);
        ArgumentNullException.ThrowIfNull(random);

        List<object> rows = new(count);

        for (int i = 0; i < count; i++)
        {
            string brand = Pick(Brands, random);
            string modelFamily = Pick(ModelFamilies[brand], random);
            float storageGb = Pick([64f, 128f, 256f, 512f], random);
            float ramGb = Pick([4f, 6f, 8f, 12f, 16f], random);
            float batteryHealth = random.Next(75, 101);
            string condition = Pick(Conditions, random);
            float releaseYear = GuessReleaseYear(modelFamily);

            float price = CalculatePhonePrice(
                brand,
                modelFamily,
                storageGb,
                ramGb,
                batteryHealth,
                condition,
                releaseYear,
                random);

            rows.Add(new PhoneDatasetRow
            {
                Brand = brand,
                ModelFamily = modelFamily,
                StorageGb = storageGb,
                RamGb = ramGb,
                BatteryHealth = batteryHealth,
                Condition = condition,
                ReleaseYear = releaseYear,
                Price = price
            });
        }

        return rows;
    }

    private static float CalculatePhonePrice(
        string brand,
        string modelFamily,
        float storageGb,
        float ramGb,
        float batteryHealth,
        string condition,
        float releaseYear,
        Random random)
    {
        float price = 120f;

        price += brand switch
        {
            "Apple" => 350f,
            "Samsung" => 220f,
            "Google" => 180f,
            "OnePlus" => 140f,
            "Xiaomi" => 90f,
            _ => 0f
        };

        price += storageGb * 0.9f;
        price += ramGb * 18f;
        price += (batteryHealth - 75f) * 9f;
        price += (releaseYear - 2020f) * 85f;

        if (modelFamily.Contains("Pro", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("S24", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("15", StringComparison.OrdinalIgnoreCase))
        {
            price += 120f;
        }

        price += condition switch
        {
            "New" => 180f,
            "As New" => 100f,
            "Refurbished" => 40f,
            "Used" => -60f,
            _ => 0f
        };

        price += (float)((random.NextDouble() - 0.5) * 80.0);

        return MathF.Max(80f, MathF.Round(price, 2));
    }

    private static float GuessReleaseYear(string modelFamily)
    {
        if (modelFamily.Contains("15", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("S24", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("Pixel 8", StringComparison.OrdinalIgnoreCase))
        {
            return 2024;
        }

        if (modelFamily.Contains("14", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("S23", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("Pixel 7", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("OnePlus 11", StringComparison.OrdinalIgnoreCase))
        {
            return 2023;
        }

        if (modelFamily.Contains("13", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("S22", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("Pixel 6", StringComparison.OrdinalIgnoreCase) ||
            modelFamily.Contains("OnePlus 10", StringComparison.OrdinalIgnoreCase))
        {
            return 2022;
        }

        return 2021;
    }

    private static T Pick<T>(IReadOnlyList<T> values, Random random)
        => values[random.Next(values.Count)];
}