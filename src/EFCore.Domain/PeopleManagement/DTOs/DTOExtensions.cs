﻿namespace EFCore.Domain.PeopleManagement.DTOs;

public static class DTOExtensions
{
    public static PersonDTO ToModel(this Person person)
        => new PersonDTO(((IHaveId<int>)person).Id, person.Name.ToModel(),
                         person.DeliveryAddress?.ToModel(),
                         person.InvoiceAddress?.ToModel());
    public static AddressDTO ToModel(this Address address)
        => new AddressDTO(address.GetType().Name.Replace("Address",""),
                          address.AddressLine1, address.AddressLine2,
                          address.PostalCode, address.City,
                          address.Country, address.IsCurrent);

    public static NameDTO ToModel(this Name name)
        => new NameDTO(name.FirstName, name.LastName);
}
