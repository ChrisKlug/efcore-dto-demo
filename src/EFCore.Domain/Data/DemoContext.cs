using Microsoft.EntityFrameworkCore;

namespace EFCore.Domain.Data;

public class DemoContext : DbContext
{
    public DemoContext() { }
    public DemoContext(DbContextOptions<DemoContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
