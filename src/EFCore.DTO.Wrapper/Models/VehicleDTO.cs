using EFCore.DTO.Wrapper.Entities;

namespace EFCore.DTO.Wrapper.Models;

public record VehicleDTO(string VIN, VehicleOwnerDTO? Owner, VehicleOwnerDTO[] PreviousOwners)
{
    public static VehicleDTO Create(Vehicle vehicle)
        => new VehicleDTO(vehicle.VIN,
                            vehicle.CurrentOwner != null ? VehicleOwnerDTO.Create(vehicle.CurrentOwner) : null, 
                            vehicle.PreviousOwners.Select(VehicleOwnerDTO.Create).OrderBy(x => x.To).ToArray());
}
