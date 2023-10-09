using EFCore.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Domain.VehicleManagement.Data;

public static class VehicleManagementContextExtensions
{
    public static Task<Vehicle?> VehicleWithVIN(this DemoContext context, VIN vin, bool asNoTracking = false)
    {
        IQueryable<Vehicle> query = context.Set<Vehicle>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(x => x.VIN == vin);
    }
    public static Task<Vehicle?> VehicleWithId(this DemoContext context, int id, bool asNoTracking = false)
    {
        IQueryable<Vehicle> query = context.Set<Vehicle>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(x => EF.Property<int>(x, "Id") == id);
    }

    public static Task<Person?> PersonWithId(this DemoContext context, int id, bool asNoTracking = false)
    {
        IQueryable<Person> query = context.Set<Person>();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return query.FirstOrDefaultAsync(x => x.Id == id);
    }
}