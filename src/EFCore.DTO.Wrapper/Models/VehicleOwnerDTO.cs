using EFCore.DTO.Wrapper.Entities;

namespace EFCore.DTO.Wrapper.Models;

public record VehicleOwnerDTO(int Id, NameDTO Name, DateTime From, DateTime? To)
{
    public static VehicleOwnerDTO Create(Owner owner)
        => new VehicleOwnerDTO(owner.Id, new NameDTO(owner.FirstName, owner.LastName), owner.From, owner.To);
}
