using System.Globalization;
using System.Text;
using PriceWise.DatasetGenerator.Models;
using PriceWise.DatasetGenerator.Pricing;

namespace PriceWise.DatasetGenerator.Generators;

/// <summary>
/// Generates synthetic laptop datasets for ML training.
/// </summary>
public sealed class LaptopDatasetGenerator
{
    private static readonly string[] Brands =
    {
        "Dell", "HP", "Lenovo", "Acer", "Asus", "MSI", "Apple"
    };

    private static readonly string[] IntelCpus =
    {
        "i3", "i5", "i7", "i9"
    };

    private static readonly string[] AppleCpus =
    {
        "M1", "M2", "M3"
    };

    private static readonly int[] RamOptions =
    {
        8, 16, 32, 64
    };

    private static readonly int[] StorageOptions =
    {
        256, 512, 1024, 2048
    };

    private static readonly string[] IntegratedGpuOnly =
    {
        "Integrated"
    };

    private static readonly string[] StandardGpuOptions =
    {
        "Integrated", "RTX2050", "RTX3050", "RTX3060", "RTX4050", "RTX4060", "RTX4070"
    };

    private static readonly string[] HighEndGpuOptions =
    {
        "RTX3050", "RTX3060", "RTX4050", "RTX4060", "RTX4070", "RTX4080"
    };

    /// <summary>
    /// Generates a collection of laptop rows.
    /// </summary>
    public IReadOnlyList<LaptopDatasetRow> Generate(int count, int seed = 42)
    {
        Random? random = new Random(seed);
        List<LaptopDatasetRow>? rows = new List<LaptopDatasetRow>(capacity: count);

        for (int i = 0; i < count; i++)
        {
            LaptopDatasetRow row = GenerateOne(random);
            rows.Add(row);
        }

        return rows;
    }

    /// <summary>
    /// Writes the generated dataset to a CSV file.
    /// </summary>
    public void WriteCsv(string outputPath, IEnumerable<LaptopDatasetRow> rows)
    {
        string? directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        StringBuilder? sb = new StringBuilder();
        sb.AppendLine("Brand,Cpu,RamGb,StorageGb,Gpu,Price");

        foreach (LaptopDatasetRow row in rows)
        {
            sb.Append(row.Brand).Append(',')
              .Append(row.Cpu).Append(',')
              .Append(row.RamGb).Append(',')
              .Append(row.StorageGb).Append(',')
              .Append(row.Gpu).Append(',')
              .Append(row.Price.ToString(CultureInfo.InvariantCulture))
              .AppendLine();
        }

        File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
    }

    private static LaptopDatasetRow GenerateOne(Random random)
    {
        string brand = PickBrand(random);
        string cpu = PickCpu(brand, random);
        int ramGb = PickRam(cpu, brand, random);
        int storageGb = PickStorage(cpu, ramGb, brand, random);
        string gpu = PickGpu(brand, cpu, ramGb, random);

        decimal price = LaptopPricingRules.CalculatePrice(
            brand,
            cpu,
            ramGb,
            storageGb,
            gpu,
            random);

        return new LaptopDatasetRow
        {
            Brand = brand,
            Cpu = cpu,
            RamGb = ramGb,
            StorageGb = storageGb,
            Gpu = gpu,
            Price = price
        };
    }

    private static string PickBrand(Random random)
    {
        return Brands[random.Next(Brands.Length)];
    }

    private static string PickCpu(string brand, Random random)
    {
        if (brand == "Apple")
        {
            return AppleCpus[random.Next(AppleCpus.Length)];
        }

        return IntelCpus[random.Next(IntelCpus.Length)];
    }

    private static int PickRam(string cpu, string brand, Random random)
    {
        if (brand == "Apple")
        {
            int[] appleRam = { 8, 16, 24, 32 };
            return appleRam[random.Next(appleRam.Length)];
        }

        if (cpu == "i3")
        {
            int[] low = { 8, 16 };
            return low[random.Next(low.Length)];
        }

        if (cpu == "i9")
        {
            int[] high = { 16, 32, 64 };
            return high[random.Next(high.Length)];
        }

        return RamOptions[random.Next(RamOptions.Length)];
    }

    private static int PickStorage(string cpu, int ramGb, string brand, Random random)
    {
        if (brand == "Apple")
        {
            int[] appleStorage = { 256, 512, 1024, 2048 };
            return appleStorage[random.Next(appleStorage.Length)];
        }

        if (cpu == "i3")
        {
            int[] low = { 256, 512 };
            return low[random.Next(low.Length)];
        }

        if (ramGb >= 32)
        {
            int[] high = { 512, 1024, 2048 };
            return high[random.Next(high.Length)];
        }

        return StorageOptions[random.Next(StorageOptions.Length)];
    }

    private static string PickGpu(string brand, string cpu, int ramGb, Random random)
    {
        if (brand == "Apple")
        {
            return "Integrated";
        }

        if (cpu == "i3")
        {
            return "Integrated";
        }

        if (brand == "MSI")
        {
            return HighEndGpuOptions[random.Next(HighEndGpuOptions.Length)];
        }

        if (cpu == "i9" || ramGb >= 32)
        {
            return HighEndGpuOptions[random.Next(HighEndGpuOptions.Length)];
        }

        if (cpu == "i5" && ramGb <= 8)
        {
            return StandardGpuOptions[random.Next(3)];
        }

        return StandardGpuOptions[random.Next(StandardGpuOptions.Length)];
    }
}