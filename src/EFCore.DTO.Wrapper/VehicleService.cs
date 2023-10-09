using EFCore.DTO.Data;
using EFCore.DTO.Wrapper.Entities;
using EFCore.DTO.Wrapper.Exceptions;
using EFCore.DTO.Wrapper.Extensions;
using EFCore.DTO.Wrapper.Models;

namespace EFCore.DTO.Wrapper;

public class VehicleService
{
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

        var existing = await context.VehicleWithVIN(vin, true);
        if (existing != null)
        {
            return ServiceResult.Fail<VehicleDTO>(new DuplicateVinException());
        }

        var owner = await context.PersonWithId(personId);
        if (owner == null)
        {
            return ServiceResult.Fail<VehicleDTO>(new PersonNotFoundException());
        }

        try
        {
            var vehicle = Vehicle.Create(vin, owner);
            context.Add((Data.Models.Vehicle)vehicle);
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
        var vehicle = await context.VehicleWithVIN(vin, true);

        return vehicle != null ?
                    ServiceResult.Success(VehicleDTO.Create(vehicle)) :
                    ServiceResult.Fail<VehicleDTO>(new VehicleNotFoundException());
    }

    public async Task<ServiceResult<VehicleDTO>> GetVehicleById(int id)
    {
        var vehicle = await context.VehicleWithId(id, true);

        return vehicle != null ?
                    ServiceResult.Success(VehicleDTO.Create(vehicle)) :
                    ServiceResult.Fail<VehicleDTO>(new VehicleNotFoundException());
    }

    public async Task<ServiceResult<VehicleOwnerDTO>> GetCurrentOwnerByVin(string vin)
    {
        var vehicle = await context.VehicleWithVIN(vin, true);
        if (vehicle == null)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new VehicleNotFoundException());
        }

        if (vehicle.CurrentOwner == null)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new OwnerNotFoundException());
        }

        return ServiceResult.Success(VehicleOwnerDTO.Create(vehicle.CurrentOwner));
    }

    public async Task<ServiceResult<VehicleOwnerDTO>> SetCurrentOwner(string vin, int personId)
    {
        var vehicle = await context.VehicleWithVIN(vin);
        if (vehicle == null)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new VehicleNotFoundException());
        }

        var newOwner = await context.PersonWithId(personId, includeAddresses: false);
        if (newOwner == null)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(new PersonNotFoundException());
        }

        try
        {
            vehicle.SetOwner(newOwner);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail<VehicleOwnerDTO>(ex);
        }


        return ServiceResult.Success(VehicleOwnerDTO.Create(vehicle.CurrentOwner!));
    }
}
