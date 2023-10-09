using EFCore.DTO.Data.Models;

namespace EFCore.DTO.Raw.Models;

public record VehicleOwnerDTO(int Id, string FirstName, string LastName, DateTime From, DateTime? To)
{
    public static VehicleOwnerDTO Create(VehicleOwner vehicleOwner)
        => new VehicleOwnerDTO(vehicleOwner.Person.Id, vehicleOwner.Person.FirstName, vehicleOwner.Person.LastName, vehicleOwner.From, vehicleOwner.To);
}
