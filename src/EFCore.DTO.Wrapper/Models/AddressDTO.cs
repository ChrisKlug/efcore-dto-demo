using EFCore.DTO.Wrapper.Entities;

namespace EFCore.DTO.Wrapper.Models;

public record AddressDTO(string Type, string AddressLine1, string? AddressLine2, string PostalCode, string City, string Country, bool IsCurrent)
{
    public static AddressDTO? Create(Address? address)
        => address != null ? new AddressDTO(address.Type,
                                            address.AddressLine1, address.AddressLine2, 
                                            address.PostalCode, address.City, 
                                            address.Country, address.IsCurrent)
                             : null;
}
