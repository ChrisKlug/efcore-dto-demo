using EFCore.DTO.Data;
using EFCore.DTO.Wrapper.Entities;
using Microsoft.EntityFrameworkCore;

namespace EFCore.DTO.Wrapper.Extensions;

public static class VehicleRegistryContextExtensions
{
    public static async Task<Vehicle?> VehicleWithVIN(this VehicleRegistryContext context, string vin, bool asNoTracking = false)
    {
        IQueryable<Data.Models.Vehicle> query = context.Vehicles
                                    .Include(x => x.Owners)
                                    .ThenInclude(x => x.Person);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        var vehicle = await query.SingleOrDefaultAsync(x => x.VIN == vin);

        return vehicle != null ? Vehicle.Create(vehicle) : null;
    }
    public static async Task<Vehicle?> VehicleWithId(this VehicleRegistryContext context, int id, bool asNoTracking = false)
    {
        IQueryable<Data.Models.Vehicle> query = context.Vehicles
                                    .Include(x => x.Owners)
                                    .ThenInclude(x => x.Person);
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        var vehicle = await query.SingleOrDefaultAsync(x => x.Id == id);

        return vehicle != null ? Vehicle.Create(vehicle) : null;
    }
    public static async Task<Person?> PersonWithId(this VehicleRegistryContext context, int id, bool includeAddresses = true, bool asNoTracking = false)
    {
        IQueryable<Data.Models.Person> query = context.People;
        if (includeAddresses)
        {
            query = query.Include(x => x.Addresses);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        var person = await query.SingleOrDefaultAsync(x => x.Id == id);

        return person != null ? Person.Create(person) : null;
    }
}
