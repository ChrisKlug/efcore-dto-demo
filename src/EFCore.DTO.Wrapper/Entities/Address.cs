namespace EFCore.DTO.Wrapper.Entities;

public class Address
{
    private readonly Data.Models.Address address;

    private Address(Data.Models.Address address)
    {
        this.address = address;
    }

    public static Address? Create(Data.Models.Address? address) 
        => address != null ? new Address(address) : null;

    public string Type => address.Type.ToString();
    public string AddressLine1 => address.AddressLine1;
    public string? AddressLine2 => address.AddressLine2;
    public string PostalCode => address.PostalCode;
    public string City => address.City;
    public string Country => address.Country;
    public bool IsCurrent => address.IsCurrent;
}
