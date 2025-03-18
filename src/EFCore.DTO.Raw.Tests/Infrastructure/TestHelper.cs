using EFCore.DTO.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;

namespace EFCore.DTO.Raw.Tests.Infrastructure;

internal class TestHelper
{
    public static VehicleRegistryContext GetContext()
    {
        var options = new DbContextOptionsBuilder<VehicleRegistryContext>()
                            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
                            .UseSqlServer(GetConnectionString());
        return new VehicleRegistryContext(options.Options);
    }

    private static string GetConnectionString()
    {
        return new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetConnectionString("SQL")!;
    }
}
