using EFCore.DTO.Data.Models;

namespace EFCore.DTO.Raw.Models;

public record AddressDTO(int Id, string Type, string AddressLine1, string? AddressLine2, string PostalCode, string City, string Country, bool IsCurrent)
{
    public static AddressDTO Create(Address address)
        => new AddressDTO(address.Id, address.Type.ToString(), address.AddressLine1, address.AddressLine2, address.PostalCode, address.City, address.Country, address.IsCurrent);
}
