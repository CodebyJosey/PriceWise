namespace Valora.Infrastructure.Persistence.Identity;

/// <summary>
/// Central place for application role names.
/// </summary>
public static class ApplicationRoles
{
    /// <summary>
    /// Administrator role.
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Seller role.
    /// </summary>
    public const string Seller = "Seller";

    /// <summary>
    /// Buyer role.
    /// </summary>
    public const string Buyer = "Buyer";

    public static readonly string[] All =
    [
        Admin,
        Seller,
        Buyer
    ];
}