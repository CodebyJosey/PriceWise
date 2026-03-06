namespace Valora.Api.Contracts.Auth;

/// <summary>
/// Request contract for user login.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}