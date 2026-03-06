namespace Valora.Api.Contracts.Auth;

/// <summary>
/// Request contract for user registration.
/// </summary>
public sealed class RegisterRequest
{
    /// <summary>
    /// Gets or sets the display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the requested role.
    /// Defaults to Buyer.
    /// </summary>
    public string Role { get; set; } = "Buyer";
}