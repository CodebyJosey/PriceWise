namespace Valora.DatasetGenerator.Abstractions;

/// <summary>
/// Contract for a dataset generator category.
/// </summary>
public interface IDatasetCategoryGenerator
{
    /// <summary>
    /// Gets the unique category key.
    /// Example: "laptops" or "phones".
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the output CSV file name.
    /// Example: "laptops.csv".
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// Generates dataset rows for this category.
    /// </summary>
    /// <param name="count">Number of rows to generate.</param>
    /// <param name="random">Random instance.</param>
    /// <returns>The generated rows.</returns>
    IReadOnlyList<object> GenerateRows(int count, Random random);
}