namespace EFCore.Domain.VehicleManagement.DTOs;

public record VehicleDTO(string VIN, OwnerDTO? Owner, OwnerDTO[] PreviousOwners);

public record NameDTO(string FirstName, string LastName);

public record OwnerDTO(int Id, NameDTO Name, DateTime From, DateTime? To);
