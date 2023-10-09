using EFCore.DTO.Data;
using EFCore.DTO.Data.Models;
using EFCore.DTO.Raw.Exceptions;
using EFCore.DTO.Raw.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace EFCore.DTO.Raw;

public class VehicleService
{
    private static Regex VinRegex = new Regex("[A-HJ-NPR-Z0-9]{17}");
    private readonly VehicleRegistryContext context;

    public VehicleService(VehicleRegistryContext context)
    {
        this.context = context;
    }

    public async Task<ServiceResult<VehicleDTO>> AddVehicle(string vin, int personId)
    {
        if (string.IsNullOrWhiteSpace(vin))
        {
            return ServiceResult.Fail<VehicleDTO>(new ArgumentNullException(nameof(vin)));
        }

        if (!VinRegex.IsMatch(vin))
        {
            return ServiceResult.Fail<VehicleDTO>(new InvalidVinException());
        }

        var existing = await context.Vehicles
                                    .Include(x => x.Owners)
                                    .ThenInclude(x => x.Person)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.VIN == vin);

        if (existing != null)
        {
            return ServiceResult.Fail<VehicleDTO>(new DuplicateVinException());
        }

        var owner = await context.People.FindAsync(personId);
        if (owner == null)
        {
            return ServiceResult.Fail<VehicleDTO>(new PersonNotFoundException());
        }

        var vehicle = new Vehicle
        {
            VIN = vin
        };

        vehicle.Owners = new List<VehicleOwner> {
                new VehicleOwner {
                    Vehicle = vehicle,
                    Person = owner,
                    From = DateTime.Today
                }
            };

        try
        {
            context.Add(vehicle);
            await context.SaveChangesAsync();
            return ServiceResult.Success(VehicleDTO.Create(vehicle));
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail<VehicleDTO>(ex);
        }
    }

    public async Task<ServiceResult<VehicleDTO>> GetVehicleByVin(string vin)
    {
        var vehicle = await context.Vehicles
                                    .Include(x => x.Owners)
                                    .ThenInclude(x => x.Person)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.VIN == vin);

        return vehicle != null ? 
                    ServiceResult.Success(VehicleDTO.Create(vehicle)) : 
                    ServiceResult.Fail<VehicleDTO>(new VehicleNotFoundException());
    }

    public async Task<ServiceResult<VehicleDTO>> GetVehicleById(int id)
    {
        var vehicle = await context.Vehicles
                                    .Include(x => x.Owners)
                                    .ThenInclude(x => x.Person)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.Id == id);

        return vehicle != null ?
                    ServiceResult.Success(VehicleDTO.Create(vehicle)) :
                    ServiceResult.Fail<VehicleDTO>(new VehicleNotFoundException());
    }

    public async Task<ServiceResult<VehicleOwnerDTO>> GetCurrentOwnerByVin(string vin)
    {
        var vehicle = await context.Vehicles
                                    .Include(x => x.Owners)
                                    .ThenInclude(x => x.Person)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.VIN == vin);
        if (vehicle == null)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new VehicleNotFoundException());
        }

        var currentOwner = vehicle.Owners.FirstOrDefault(x => x.To == null);
        if (currentOwner == null)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new OwnerNotFoundException());
        }

        return ServiceResult.Success(VehicleOwnerDTO.Create(currentOwner));
    }

    public async Task<ServiceResult<VehicleOwnerDTO>> SetCurrentOwner(string vin, int personId)
    {
        var vehicle = await context.Vehicles
                                    .Include(x => x.Owners)
                                    .ThenInclude(x => x.Person)
                                    .FirstOrDefaultAsync(x => x.VIN == vin);

        if (vehicle == null)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new VehicleNotFoundException());
        }

        var currentOwner = vehicle.Owners.FirstOrDefault(x => x.To == null);
        if (currentOwner?.Person.Id == personId)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new DuplicateOwnerException());
        }

        var newOwner = await context.People.FindAsync(personId);
        if (newOwner == null)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new PersonNotFoundException());
        }

        if (currentOwner != null)
        {
            currentOwner.To = DateTime.Today;
        }

        var pv = new Data.Models.VehicleOwner
        {
            Person = newOwner,
            Vehicle = vehicle,
            From = DateTime.Today
        };

        vehicle.Owners.Add(pv);

        await context.SaveChangesAsync();

        return ServiceResult.Success(VehicleOwnerDTO.Create(pv));
    }
}
