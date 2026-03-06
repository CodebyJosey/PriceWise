using System.Globalization;
using System.Reflection;
using System.Text;

namespace PriceWise.DatasetGenerator.Services;

/// <summary>
/// Writes dataset rows to CSV using public properties as columns.
/// </summary>
public sealed class CsvDatasetWriter
{
    /// <summary>
    /// Writes rows to a CSV file.
    /// </summary>
    /// <param name="outputPath">The output file path.</param>
    /// <param name="rows">The rows to write.</param>
    public void Write(string outputPath, IReadOnlyList<object> rows)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        ArgumentNullException.ThrowIfNull(rows);

        string? directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using StreamWriter writer = new(outputPath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        if (rows.Count == 0)
        {
            return;
        }

        Type rowType = rows[0].GetType();
        PropertyInfo[] properties = rowType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToArray();

        writer.WriteLine(string.Join(",", properties.Select(property => Escape(property.Name))));

        foreach (object row in rows)
        {
            string line = string.Join(",", properties.Select(property =>
            {
                object? value = property.GetValue(row);
                return Escape(FormatValue(value));
            }));

            writer.WriteLine(line);
        }
    }

    private static string FormatValue(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return value switch
        {
            float f => f.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            decimal m => m.ToString(CultureInfo.InvariantCulture),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }

    private static string Escape(string value)
    {
        if (!value.Contains(',') && !value.Contains('"') && !value.Contains('\n') && !value.Contains('\r'))
        {
            return value;
        }

        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}