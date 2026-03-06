namespace Valora.Api.Contracts.Auth;

/// <summary>
/// Response returned after successful authentication.
/// </summary>
public sealed class AuthResponse
{
    /// <summary>
    /// Gets or sets the JWT access token.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Gets or sets the UTC expiration timestamp.
    /// </summary>
    public DateTime ExpiresAtUtc { get; init; }

    /// <summary>
    /// Gets or sets the user id.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets or sets the email.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// Gets or sets the user roles.
    /// </summary>
    public required IReadOnlyCollection<string> Roles { get; init; }
}