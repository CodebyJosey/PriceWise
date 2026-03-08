using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using Valora.Application.Abstractions.Logging;

namespace Valora.Api.Middleware;

/// <summary>
/// Middleware for request/response audit logging.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestLoggingMiddleware"/> class.
    /// </summary>
    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
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
        Stopwatch stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        Guid? userId = TryGetUserId(context.User);

        string[] roles = context.User.Claims
            .Where(x => x.Type == ClaimTypes.Role)
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        string details = JsonSerializer.Serialize(new
        {
            method = context.Request.Method,
            path = context.Request.Path.Value,
            queryString = context.Request.QueryString.Value,
            statusCode = context.Response.StatusCode,
            elapsedMs = stopwatch.ElapsedMilliseconds,
            traceIdentifier = context.TraceIdentifier,
            roles,
            remoteIp = context.Connection.RemoteIpAddress?.ToString(),
            userAgent = context.Request.Headers.UserAgent.ToString()
        });

        _logger.LogInformation(
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);

        await auditLogService.WriteAsync(
            userId,
            action: "HttpRequest",
            entityType: "Request",
            entityId: context.TraceIdentifier,
            details: details,
            cancellationToken: context.RequestAborted);
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