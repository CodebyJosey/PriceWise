namespace Valora.Application.Abstractions.PricePrediction;

/// <summary>
/// Non-ML abstraction for a price prediction category.
/// A category represents a prediction domain such as laptops or phones.
/// </summary>
public interface IPricePredictionCategory
{
    /// <summary>
    /// Gets the unique key of the category.
    /// Example: "laptops", "phones".
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the dataset file name for this category.
    /// Example: "laptops.csv".
    /// </summary>
    string DatasetFileName { get; }

    /// <summary>
    /// Gets the model file name for this category.
    /// Example: "laptop-price-model.zip".
    /// </summary>
    string ModelFileName { get; }

    /// <summary>
    /// Gets the CLR type of the public features contract for this category.
    /// </summary>
    Type FeaturesType { get; }

    /// <summary>
    /// Gets the CLR type of the ML training row for this category.
    /// </summary>
    Type TrainingRowType { get; }

    /// <summary>
    /// Gets the CLR type of the ML prediction output for this category.
    /// </summary>
    Type PredictionType { get; }

    /// <summary>
    /// Maps an incoming features object to the corresponding training row object.
    /// </summary>
    /// <param name="features">The features object.</param>
    /// <returns>The mapped training row object.</returns>
    object MapFeaturesToTrainingRow(object features);

    /// <summary>
    /// Creates a sample features object for sanity checks.
    /// </summary>
    /// <returns>A sample features object.</returns>
    object CreateSanityFeatures();

    /// <summary>
    /// Creates a sample training row object for sanity checks.
    /// </summary>
    /// <returns>A sample training row object.</returns>
    object CreateSanityTrainingRow();
}

/// <summary>
/// Strongly typed abstraction for a price prediction category.
/// </summary>
/// <typeparam name="TFeatures">The public input features type.</typeparam>
/// <typeparam name="TTrainingRow">The ML training row type.</typeparam>
/// <typeparam name="TPrediction">The ML prediction type.</typeparam>
public interface IPricePredictionCategory<TFeatures, TTrainingRow, TPrediction> : IPricePredictionCategory
    where TFeatures : class
    where TTrainingRow : class, new()
    where TPrediction : class, new()
{
    /// <summary>
    /// Maps an incoming features object to the corresponding training row object.
    /// </summary>
    /// <param name="features">The features object.</param>
    /// <returns>The mapped training row object.</returns>
    new TTrainingRow MapFeaturesToTrainingRow(TFeatures features);

    /// <summary>
    /// Creates a sample features object for sanity checks.
    /// </summary>
    /// <returns>A sample features object.</returns>
    new TFeatures CreateSanityFeatures();

    /// <summary>
    /// Creates a sample training row object for sanity checks.
    /// </summary>
    /// <returns>A sample training row object.</returns>
    new TTrainingRow CreateSanityTrainingRow();
}