using Valora.DatasetGenerator.Abstractions;
using Valora.DatasetGenerator.Rows;

namespace Valora.DatasetGenerator.Generators;

/// <summary>
/// Generates tablet dataset rows.
/// </summary>
public sealed class TabletDatasetCategoryGenerator : IDatasetCategoryGenerator
{
    private static readonly string[] Brands =
    [
        "Apple", "Samsung", "Microsoft", "Lenovo", "Xiaomi"
    ];

    private static readonly Dictionary<string, string[]> ModelFamilies = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Apple"] = ["iPad", "iPad Air", "iPad Pro", "iPad Mini"],
        ["Samsung"] = ["Galaxy Tab A9", "Galaxy Tab S8", "Galaxy Tab S9", "Galaxy Tab S9 FE"],
        ["Microsoft"] = ["Surface Go", "Surface Pro 8", "Surface Pro 9"],
        ["Lenovo"] = ["Tab M10", "Tab P11", "Yoga Tab 11"],
        ["Xiaomi"] = ["Pad 5", "Pad 6", "Redmi Pad"]
    };

    private static readonly string[] Conditions =
    [
        "Used", "Refurbished", "As New", "New"
    ];

    private static readonly string[] ConnectivityOptions =
    [
        "WiFi", "WiFi+Cellular"
    ];

    /// <inheritdoc />
    public string Key => "tablets";

    /// <inheritdoc />
    public string FileName => "tablets.csv";

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
            float screenSizeInch = Pick([8.3f, 10.1f, 10.9f, 11f, 12.4f, 12.9f], random);
            string condition = Pick(Conditions, random);
            float releaseYear = random.Next(2020, 2027);
            string connectivity = Pick(ConnectivityOptions, random);

            float price = CalculateTabletPrice(
                brand,
                modelFamily,
                storageGb,
                ramGb,
                screenSizeInch,
                condition,
                releaseYear,
                connectivity,
                random);

            rows.Add(new TabletDatasetRow
            {
                Brand = brand,
                ModelFamily = modelFamily,
                StorageGb = storageGb,
                RamGb = ramGb,
                ScreenSizeInch = screenSizeInch,
                Condition = condition,
                ReleaseYear = releaseYear,
                Connectivity = connectivity,
                Price = price
            });
        }

        return rows;
    }

    private static float CalculateTabletPrice(
        string brand,
        string modelFamily,
        float storageGb,
        float ramGb,
        float screenSizeInch,
        string condition,
        float releaseYear,
        string connectivity,
        Random random)
    {
        float price = 180f;

        price += brand switch
        {
            "Apple" => 320f,
            "Microsoft" => 280f,
            "Samsung" => 220f,
            "Lenovo" => 100f,
            "Xiaomi" => 90f,
            _ => 0f
        };

        price += modelFamily switch
        {
            "iPad Pro" => 420f,
            "iPad Air" => 220f,
            "iPad Mini" => 120f,
            "Surface Pro 9" => 380f,
            "Surface Pro 8" => 300f,
            "Surface Go" => 120f,
            "Galaxy Tab S9" => 280f,
            "Galaxy Tab S8" => 220f,
            "Galaxy Tab S9 FE" => 160f,
            "Galaxy Tab A9" => 60f,
            "Pad 6" => 110f,
            "Pad 5" => 80f,
            _ => 0f
        };

        price += storageGb * 0.85f;
        price += ramGb * 20f;
        price += (screenSizeInch - 8f) * 22f;
        price += (releaseYear - 2020f) * 75f;

        price += connectivity switch
        {
            "WiFi+Cellular" => 140f,
            _ => 0f
        };

        price += condition switch
        {
            "New" => 180f,
            "As New" => 100f,
            "Refurbished" => 40f,
            "Used" => -70f,
            _ => 0f
        };

        price += (float)((random.NextDouble() - 0.5) * 90.0);

        return MathF.Max(100f, MathF.Round(price, 2));
    }

    private static T Pick<T>(IReadOnlyList<T> values, Random random)
        => values[random.Next(values.Count)];
}