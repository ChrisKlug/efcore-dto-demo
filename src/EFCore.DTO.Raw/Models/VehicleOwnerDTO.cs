using EFCore.DTO.Data.Models;

namespace EFCore.DTO.Raw.Models;

public record VehicleOwnerDTO(int Id, NameDTO Name, DateTime From, DateTime? To)
{
    public static VehicleOwnerDTO Create(VehicleOwner vehicleOwner)
        => new VehicleOwnerDTO(vehicleOwner.Person.Id, new NameDTO(vehicleOwner.Person.FirstName, vehicleOwner.Person.LastName), vehicleOwner.From, vehicleOwner.To);
}
