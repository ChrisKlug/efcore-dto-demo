namespace EFCore.Domain.PeopleManagement.DTOs;

public record PersonDTO(int Id, NameDTO Name, AddressDTO? DeliveryAddress, AddressDTO? InvoiceAddress);

public record NameDTO(string FirstName, string LastName);

public record AddressDTO(string Type, string AddressLine1, string? AddressLine2, string PostalCode, string City, string Country, bool IsCurrent);

