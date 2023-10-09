using EFCore.DTO.Wrapper.Entities;

namespace EFCore.DTO.Wrapper.Models;

public record PersonDTO(int Id, string FirstName, string LastName, AddressDTO? DeliveryAddress, AddressDTO? InvoiceAddress)
{
    public static PersonDTO Create(Person person)
        => new PersonDTO(person.Id, person.FirstName, person.LastName, 
                         AddressDTO.Create(person.DeliveryAddress), 
                         AddressDTO.Create(person.InvoiceAddress));
}
