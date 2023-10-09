using EFCore.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Domain.PeopleManagement.Data;

public static class DemoContextExtensions
{
    public static Task<Person?> PersonWithId(this DemoContext context, int id, bool asNoTracking = false)
    {
        IQueryable<Person> query = context.Set<Person>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query.SingleOrDefaultAsync(x => EF.Property<int>(x, "id") == id);
    }
}
