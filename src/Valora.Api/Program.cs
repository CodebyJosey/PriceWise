using Microsoft.EntityFrameworkCore;
using Valora.Api.Extensions;
using Valora.Infrastructure.Persistence;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValoraPersistence(builder.Configuration);
builder.Services.AddValoraServices(builder.Environment.ContentRootPath);

WebApplication? app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await ApplyDatabaseMigrationsAsync(app);

app.Run();

static async Task ApplyDatabaseMigrationsAsync(WebApplication app)
{
    using IServiceScope scope = app.Services.CreateScope();
    ValoraDbContext dbContext = scope.ServiceProvider.GetRequiredService<ValoraDbContext>();

    await dbContext.Database.MigrateAsync();
}