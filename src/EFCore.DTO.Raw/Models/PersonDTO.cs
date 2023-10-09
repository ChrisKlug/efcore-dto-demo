using EFCore.DTO.Data.Models;

namespace EFCore.DTO.Raw.Models;

public record PersonDTO(int Id, string FirstName, string LastName, AddressDTO? DeliveryAddress, AddressDTO? InvoiceAddress)
{
    public static PersonDTO Create(Person person, Address[] addresses)
    {
        var deliveryAddress = addresses.FirstOrDefault(x => x.IsCurrent && x.Type == AddressType.Delivery);
        var deliveryAddressDto = deliveryAddress != null ? AddressDTO.Create(deliveryAddress) : null;

        var invoiceAddress = addresses.FirstOrDefault(x => x.IsCurrent && x.Type == AddressType.Invoice);
        var invoiceAddressDto = invoiceAddress != null ? AddressDTO.Create(invoiceAddress) : null;

        return new PersonDTO(person.Id, person.FirstName, person.LastName, deliveryAddressDto, invoiceAddressDto);
    }
}
