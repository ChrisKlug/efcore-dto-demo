using EFCore.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace EFCore.Domain.Tests.Infrastructure;

internal class TestHelper
{
    public static DemoContext GetContext()
    {
        var options = new DbContextOptionsBuilder<DemoContext>()
                            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
                            .UseSqlServer(GetConnectionString());
        return new DemoContext(options.Options);
    }

    private static string GetConnectionString()
    {
        return new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetConnectionString("SQL")!;
    }
}
