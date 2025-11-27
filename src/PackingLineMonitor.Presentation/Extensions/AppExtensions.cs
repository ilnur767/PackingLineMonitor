using Microsoft.EntityFrameworkCore;
using PackingLineMonitor.Infrastructure.Database;

namespace PackingLineMonitor.Presentation.Extensions;

public static class AppExtensions
{
    public static async Task ApplyMigration(this IApplicationBuilder app)
    {
        await using var scope = app.ApplicationServices.CreateAsyncScope();

        var dbContext =
            scope.ServiceProvider.GetRequiredService<PackingLineMonitorDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}