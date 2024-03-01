using Microsoft.EntityFrameworkCore;

namespace EFCore.Domain.Data;

public class DemoContext : DbContext
{
    public DemoContext() { }
    public DemoContext(DbContextOptions<DemoContext> options)
        : base(options) { }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    base.OnConfiguring(optionsBuilder);
    //    optionsBuilder.LogTo(str => Debug.WriteLine(str));
    //    optionsBuilder.UseSqlServer(options => {
    //        options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    //    });
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}
