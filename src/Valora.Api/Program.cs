using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Valora.Api.Extensions;
using Valora.Api.Middleware;
using Valora.Infrastructure.Persistence;
using Valora.Infrastructure.Persistence.Seed;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Valora API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddValoraPersistence(builder.Configuration);
builder.Services.AddValoraServices(builder.Environment.ContentRootPath);

WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

await ApplyDatabaseMigrationsAsync(app);
await SeedIdentityAsync(app);

app.Run();

static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();
    ValoraDbContext dbContext = scope.ServiceProvider.GetRequiredService<ValoraDbContext>();

    await dbContext.Database.MigrateAsync();
}

static async Task SeedIdentityAsync(WebApplication app)
{
    await IdentitySeeder.SeedAsync(app.Services);
}