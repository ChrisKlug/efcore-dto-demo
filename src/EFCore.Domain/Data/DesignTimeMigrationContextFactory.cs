using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EFCore.Domain.Data;

public class DesignTimeMigrationContextFactory : IDesignTimeDbContextFactory<DemoContext>
{
    public DemoContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<DemoContext>();
        builder.UseSqlServer();
        return new DemoContext(builder.Options);
    }
}