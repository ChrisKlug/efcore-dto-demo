namespace EFCore.Domain.VehicleManagement.DTOs;

public record VehicleDTO(string VIN, OwnerDTO? Owner, OwnerDTO[] PreviousOwners);
public record OwnerDTO(int Id, string FirstName, string LastName, DateTime From, DateTime? To);
