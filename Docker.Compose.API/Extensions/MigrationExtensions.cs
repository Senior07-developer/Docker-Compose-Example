using Docker.Compose.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Docker.Compose.API.Extensions;

public static class MigrationExtensions
{
    public static async Task ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope serviceScope = app.ApplicationServices.CreateScope();

        await using var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        await dbContext.Database.MigrateAsync();
    }
}