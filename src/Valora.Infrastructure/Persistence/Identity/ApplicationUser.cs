using Microsoft.AspNetCore.Identity;

namespace Valora.Infrastructure.Persistence.Identity;

/// <summary>
/// Represents the ASP.NET Core Identity user for Valora.
/// This type stays in Infrastructure so Domain remains framework-free.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Gets or sets the user's display name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC creation date of the user.
    /// </summary>
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Gets or sets whether the user is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}