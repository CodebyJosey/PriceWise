namespace Valora.Domain.Entities;

/// <summary>
/// Represents a stored prediction request and result.
/// </summary>
public class PredictionRecord
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the optional user id who made the request.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the category key.
    /// </summary>
    public string CategoryKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the serialized feature payload as JSON.
    /// </summary>
    public string FeaturesJson { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the predicted price.
    /// </summary>
    public decimal PredictedPrice { get; set; }

    /// <summary>
    /// Gets or sets the model version used for this prediction.
    /// </summary>
    public int? ModelVersion { get; set; }

    /// <summary>
    /// Gets or sets the UTC creation timestamp.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}