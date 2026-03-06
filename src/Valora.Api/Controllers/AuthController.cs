using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Valora.Api.Contracts.Auth;
using Valora.Infrastructure.Persistence.Identity;

namespace Valora.Api.Controllers;

/// <summary>
/// Authentication endpoints for Valora.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="userManager">The user manager.</param>
    /// <param name="signInManager">The sign-in manager.</param>
    /// <param name="configuration">The application configuration.</param>
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="request">The register request.</param>
    /// <returns>The created user result.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DisplayName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid registration request",
                Detail = "DisplayName, Email and Password are required.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        string normalizedRole = NormalizeRole(request.Role);

        if (normalizedRole is not ApplicationRoles.Buyer and not ApplicationRoles.Seller)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid role",
                Detail = "Only Buyer or Seller can be chosen during self-registration.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        ApplicationUser? existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = $"A user with email '{request.Email}' already exists.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        ApplicationUser user = new()
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            EmailConfirmed = true,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };

        IdentityResult createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Registration failed",
                Detail = string.Join("; ", createResult.Errors.Select(x => x.Description)),
                Status = StatusCodes.Status400BadRequest
            });
        }

        IdentityResult roleResult = await _userManager.AddToRoleAsync(user, normalizedRole);
        if (!roleResult.Succeeded)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Role assignment failed",
                Detail = string.Join("; ", roleResult.Errors.Select(x => x.Description)),
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok(new
        {
            userId = user.Id,
            displayName = user.DisplayName,
            email = user.Email,
            role = normalizedRole
        });
    }

    /// <summary>
    /// Logs a user in and returns a JWT.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <returns>The auth response.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid login request",
                Detail = "Email and Password are required.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        ApplicationUser? user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Login failed",
                Detail = "Invalid email or password.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!signInResult.Succeeded)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Login failed",
                Detail = "Invalid email or password.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        IReadOnlyCollection<string> roles = (await _userManager.GetRolesAsync(user)).ToArray();
        AuthResponse response = BuildAuthResponse(user, roles);

        return Ok(response);
    }

    /// <summary>
    /// Gets the current authenticated user.
    /// </summary>
    /// <returns>The current user.</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(MeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Me()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid parsedUserId))
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "The current token is invalid.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        ApplicationUser? user = await _userManager.FindByIdAsync(parsedUserId.ToString());
        if (user is null)
        {
            return Unauthorized(new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "The current user could not be found.",
                Status = StatusCodes.Status401Unauthorized
            });
        }

        IReadOnlyCollection<string> roles = (await _userManager.GetRolesAsync(user)).ToArray();

        return Ok(new MeResponse
        {
            UserId = user.Id.ToString(),
            DisplayName = user.DisplayName,
            Email = user.Email ?? string.Empty,
            Roles = roles
        });
    }

    private AuthResponse BuildAuthResponse(ApplicationUser user, IReadOnlyCollection<string> roles)
    {
        string jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT configuration value 'Jwt:Key' was not found.");

        string issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT configuration value 'Jwt:Issuer' was not found.");

        string audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT configuration value 'Jwt:Audience' was not found.");

        int expiresMinutes = int.TryParse(_configuration["Jwt:ExpiresMinutes"], out int parsedMinutes)
            ? parsedMinutes
            : 120;

        DateTime expiresAtUtc = DateTime.UtcNow.AddMinutes(expiresMinutes);

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.DisplayName),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        ];

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtKey));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse
        {
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id.ToString(),
            DisplayName = user.DisplayName,
            Email = user.Email ?? string.Empty,
            Roles = roles
        };
    }

    private static string NormalizeRole(string role)
    {
        if (string.Equals(role, ApplicationRoles.Seller, StringComparison.OrdinalIgnoreCase))
        {
            return ApplicationRoles.Seller;
        }

        return ApplicationRoles.Buyer;
    }
}