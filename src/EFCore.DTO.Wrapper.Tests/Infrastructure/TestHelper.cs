using EFCore.DTO.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCore.DTO.Wrapper.Tests.Infrastructure;

internal class TestHelper
{
    public static VehicleRegistryContext GetContext()
    {
        var options = new DbContextOptionsBuilder<VehicleRegistryContext>()
                            .UseSqlServer(GetConnectionString());
        return new VehicleRegistryContext(options.Options);
    }

    private static string GetConnectionString()
    {
        return new ConfigurationBuilder().AddJsonFile("appSettings.json").Build().GetConnectionString("SQL")!;
    }
}
