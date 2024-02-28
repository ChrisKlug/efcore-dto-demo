using EFCore.DTO.Wrapper.Entities;

namespace EFCore.DTO.Wrapper.Models;

public record PersonDTO(int Id, NameDTO Name, AddressDTO? DeliveryAddress, AddressDTO? InvoiceAddress)
{
    public static PersonDTO Create(Person person)
        => new PersonDTO(person.Id, new NameDTO(person.Name.FirstName, person.Name.LastName), 
                         AddressDTO.Create(person.DeliveryAddress), 
                         AddressDTO.Create(person.InvoiceAddress));
}
