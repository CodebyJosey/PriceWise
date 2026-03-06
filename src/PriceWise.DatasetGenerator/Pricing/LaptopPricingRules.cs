namespace PriceWise.DatasetGenerator.Pricing;

/// <summary>
/// Contains pricing rules for synthetic laptop dataset generation.
/// </summary>
public static class LaptopPricingRules
{
    /// <summary>
    /// Calculates a synthetic but realistic laptop price based on the supplied features.
    /// </summary>
    public static decimal CalculatePrice(
        string brand,
        string cpu,
        int ramGb,
        int storageGb,
        string gpu,
        Random random
    )
    {
        decimal price = 250m;

        price += GetBrandModifier(brand);
        price += GetCpuModifier(cpu);
        price += GetRamModifier(ramGb);
        price += GetStorageModifier(storageGb);
        price += GetGpuModifier(gpu);

        price += GetCombinationBonus(brand, cpu, ramGb, storageGb, gpu);

        decimal noisePercentage = (decimal)(random.NextDouble() * 0.18 - 0.09); // -9% to +9%
        price += price * noisePercentage;

        if (price < 300m)
        {
            price = 300m;
        }

        return decimal.Round(price, 2);
    }

    private static decimal GetBrandModifier(string brand)
    {
        return brand switch
        {
            "Dell" => 110m,
            "HP" => 90m,
            "Lenovo" => 95m,
            "Acer" => 50m,
            "Asus" => 100m,
            "MSI" => 180m,
            "Apple" => 350m,
            _ => 0m
        };
    }

    private static decimal GetCpuModifier(string cpu)
    {
        return cpu switch
        {
            "i3" => 80m,
            "i5" => 180m,
            "i7" => 320m,
            "i9" => 520m,
            "M1" => 280m,
            "M2" => 420m,
            "M3" => 560m,
            _ => 0m
        };
    }

    private static decimal GetRamModifier(int ramGb)
    {
        return ramGb switch
        {
            4 => 0m,
            8 => 80m,
            16 => 180m,
            32 => 340m,
            64 => 650m,
            _ => ramGb * 10m
        };
    }

    private static decimal GetStorageModifier(int storageGb)
    {
        return storageGb switch
        {
            128 => 20m,
            256 => 60m,
            512 => 140m,
            1024 => 260m,
            2048 => 480m,
            _ => storageGb * 0.2m
        };
    }

    private static decimal GetGpuModifier(string gpu)
    {
        return gpu switch
        {
            "Integrated" => 0m,
            "RTX2050" => 180m,
            "RTX3050" => 280m,
            "RTX3060" => 400m,
            "RTX4050" => 500m,
            "RTX4060" => 650m,
            "RTX4070" => 900m,
            "RTX4080" => 1250m,
            _ => 0m
        };
    }

    private static decimal GetCombinationBonus(
        string brand,
        string cpu,
        int ramGb,
        int storageGb,
        string gpu)
    {
        decimal bonus = 0m;

        // Gaming premium
        if (gpu != "Integrated" && ramGb >= 16)
        {
            bonus += 120m;
        }

        // High-end workstation / premium class
        if ((cpu == "i9" || cpu == "M3") && ramGb >= 32 && storageGb >= 1024)
        {
            bonus += 180m;
        }

        // Apple premium behavior
        if (brand == "Apple")
        {
            bonus += 120m;

            if (gpu != "Integrated")
            {
                bonus -= 300m;
            }
        }

        // MSI gaming identity
        if (brand == "MSI" && gpu != "Integrated")
        {
            bonus += 150m;
        }

        // Budget brands get a slight reduction
        if (brand == "Acer" && gpu == "Integrated")
        {
            bonus -= 50m;
        }

        return bonus;
    }
}