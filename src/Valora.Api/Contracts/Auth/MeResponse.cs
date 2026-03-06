namespace Valora.Api.Contracts.Auth;

/// <summary>
/// Response contract for the currently authenticated user.
/// </summary>
public sealed class MeResponse
{
    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets the roles.
    /// </summary>
    public required IReadOnlyCollection<string> Roles { get; init; }
}