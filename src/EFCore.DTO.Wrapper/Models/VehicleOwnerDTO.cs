using EFCore.DTO.Wrapper.Entities;

namespace EFCore.DTO.Wrapper.Models;

public record VehicleOwnerDTO(int Id, string FirstName, string LastName, DateTime From, DateTime? To)
{
    public static VehicleOwnerDTO Create(Owner owner)
        => new VehicleOwnerDTO(owner.Id, owner.FirstName, owner.LastName, owner.From, owner.To);
}
