using EFCore.Domain.Infrastructure;
using EFCore.Domain.VehicleManagement;
using EFCore.Domain.Data;
using EFCore.Domain.VehicleManagement.DTOs;
using EFCore.Domain.VehicleManagement.Exceptions;
using EFCore.Domain.VehicleManagement.Data;

namespace EFCore.Domain;

public class VehicleService
{
    private readonly DemoContext context;

    public VehicleService(DemoContext context)
    {
        this.context = context;
    }

    public async Task<ServiceResult<VehicleDTO>> AddVehicle(string vin, int personId)
    {
        if (!VIN.TryCreate(vin, out var v))
        {
            return ServiceResult.Fail<VehicleDTO>(new InvalidVinException());
        }

        var existing = await context.VehicleWithVIN(v!, false);
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
            var vehicle = Vehicle.Create(v!, owner);
            context.Add(vehicle);
            await context.SaveChangesAsync();
            return ServiceResult.Success(vehicle.ToModel());
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail<VehicleDTO>(ex);
        }
    }

    public async Task<ServiceResult<VehicleDTO>> GetVehicleByVin(string vin)
    {
        if (!VIN.TryCreate(vin, out var v))
        {
            return ServiceResult.Fail<VehicleDTO>(new InvalidVinException());
        }

        var vehicle = await context.VehicleWithVIN(v!, true);

        return vehicle != null ?
                    ServiceResult.Success(vehicle.ToModel()) :
                    ServiceResult.Fail<VehicleDTO>(new VehicleNotFoundException());
    }

    public async Task<ServiceResult<VehicleDTO>> GetVehicleById(int id)
    {
        var vehicle = await context.VehicleWithId(id, true);

        return vehicle != null ?
                    ServiceResult.Success(vehicle.ToModel()) :
                    ServiceResult.Fail<VehicleDTO>(new VehicleNotFoundException());
    }

    public async Task<ServiceResult<OwnerDTO>> GetCurrentOwnerByVin(string vin)
    {
        if (!VIN.TryCreate(vin, out var v))
        {
            return ServiceResult.Fail<OwnerDTO>(new InvalidVinException());
        }

        var vehicle = await context.VehicleWithVIN(v!, true);
        if (vehicle == null)
        {
            return ServiceResult.Fail<OwnerDTO>(new VehicleNotFoundException());
        }

        if (vehicle.CurrentOwner == null)
        {
            return ServiceResult.Fail<OwnerDTO>(new OwnerNotFoundException());
        }

        return ServiceResult.Success(vehicle.CurrentOwner.ToModel());
    }

    public async Task<ServiceResult<OwnerDTO>> SetCurrentOwner(string vin, int personId)
    {
        if (!VIN.TryCreate(vin, out var v))
        {
            return ServiceResult.Fail<OwnerDTO>(new InvalidVinException());
        }

        var vehicle = await context.VehicleWithVIN(v!);
        if (vehicle == null)
        {
            return ServiceResult.Fail<OwnerDTO>(new VehicleNotFoundException());
        }

        var newOwner = await context.PersonWithId(personId);
        if (newOwner == null)
        {
            return ServiceResult.Fail<OwnerDTO>(new PersonNotFoundException());
        }

        try
        {
            vehicle.SetOwner(newOwner);
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail<OwnerDTO>(ex);
        }

        return ServiceResult.Success(vehicle.CurrentOwner!.ToModel());
    }
}
