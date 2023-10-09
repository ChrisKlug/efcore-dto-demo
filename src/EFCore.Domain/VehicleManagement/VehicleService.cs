using EFCore.Domain.Infrastructure;
using EFCore.Domain.VehicleManagement.DTOs;

namespace EFCore.Domain;

public class VehicleService
{
    public async Task<ServiceResult<VehicleDTO>> AddVehicle(string vin, int personId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<VehicleDTO>> GetVehicleByVin(string vin)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<VehicleDTO>> GetVehicleById(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<OwnerDTO>> GetCurrentOwnerByVin(string vin)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<OwnerDTO>> SetCurrentOwner(string vin, int personId)
    {
        throw new NotImplementedException();
    }
}
