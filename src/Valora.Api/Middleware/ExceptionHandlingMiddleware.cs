using System.Security.Claims;
using System.Text.Json;
using Valora.Application.Abstractions.Logging;

namespace Valora.Api.Middleware;

/// <summary>
/// Middleware for centralized exception handling and exception audit logging.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    public async Task InvokeAsync(
        HttpContext context,
        IAuditLogService auditLogService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Guid? userId = TryGetUserId(context.User);

            _logger.LogError(
                ex,
                "Unhandled exception for {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            string details = JsonSerializer.Serialize(new
            {
                method = context.Request.Method,
                path = context.Request.Path.Value,
                queryString = context.Request.QueryString.Value,
                exceptionType = ex.GetType().Name,
                ex.Message
            });

            try
            {
                await auditLogService.WriteAsync(
                    userId,
                    action: "UnhandledException",
                    entityType: "Request",
                    entityId: context.TraceIdentifier,
                    details: details,
                    cancellationToken: context.RequestAborted);
            }
            catch
            {
                // Intentionally swallow secondary logging failures.
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                title = "An unexpected error occurred",
                status = StatusCodes.Status500InternalServerError,
                detail = ex.Message
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private static Guid? TryGetUserId(ClaimsPrincipal user)
    {
        string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid parsed))
        {
            return null;
        }

        return parsed;
    }
}