using EFCore.DTO.Data.Models;

namespace EFCore.DTO.Raw.Models;

public record VehicleDTO(int Id, string VIN, VehicleOwnerDTO Owner, VehicleOwnerDTO[] PreviousOwners)
{
    public static VehicleDTO Create(Vehicle vehicle)
        => new VehicleDTO(vehicle.Id, vehicle.VIN, 
                            VehicleOwnerDTO.Create(vehicle.Owners.First(X => X.To == null)), 
                            vehicle.Owners.Where(x => x.To != null).Select(VehicleOwnerDTO.Create).OrderBy(x => x.To).ToArray());
}
