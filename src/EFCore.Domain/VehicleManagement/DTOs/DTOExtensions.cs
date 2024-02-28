namespace EFCore.Domain.VehicleManagement.DTOs;

public static class DTOExtensions
{
    public static VehicleDTO ToModel(this Vehicle vehicle)
        => new VehicleDTO(vehicle.VIN.Value,
                            vehicle.CurrentOwner != null ? vehicle.CurrentOwner.ToModel() : null,
                            vehicle.PreviousOwners.OrderBy(x => x.To).Select(x => x.ToModel()).ToArray());

    public static OwnerDTO ToModel(this Owner owner)
        => new OwnerDTO(owner.Id, owner.Name.ToModel(), owner.From, owner.To);
    
    public static NameDTO ToModel(this Name name)
        => new NameDTO(name.FirstName, name.LastName);
}