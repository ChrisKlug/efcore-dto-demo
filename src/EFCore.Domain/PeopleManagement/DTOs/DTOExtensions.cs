namespace EFCore.Domain.PeopleManagement.DTOs;

public static class DTOExtensions
{
    public static PersonDTO ToModel(this Person person)
        => new PersonDTO(-1, person.FirstName, person.LastName,
                         person.DeliveryAddress?.ToModel(),
                         person.InvoiceAddress?.ToModel());
    public static AddressDTO ToModel(this Address address)
        => new AddressDTO(address.GetType().Name.Replace("Address",""),
                          address.AddressLine1, address.AddressLine2,
                          address.PostalCode, address.City,
                          address.Country, address.IsCurrent);
}
