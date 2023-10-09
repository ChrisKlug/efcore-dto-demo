using EFCore.Domain.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore.Domain.Tests.Infrastructure;

internal class TestHelper
{
    public static DemoContext GetContext()
    {
        var options = new DbContextOptionsBuilder<DemoContext>()
                            .UseSqlServer(GetConnectionString());
        return new DemoContext(options.Options);
    }

    private static string GetConnectionString()
    {
        return new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetConnectionString("SQL")!;
    }
}
