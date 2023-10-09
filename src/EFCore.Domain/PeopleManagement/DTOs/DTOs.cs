namespace EFCore.Domain.PeopleManagement.DTOs;

public record PersonDTO(int Id, string FirstName, string LastName, AddressDTO? DeliveryAddress, AddressDTO? InvoiceAddress) { }

public record AddressDTO(string Type, string AddressLine1, string? AddressLine2, string PostalCode, string City, string Country, bool IsCurrent) { }

