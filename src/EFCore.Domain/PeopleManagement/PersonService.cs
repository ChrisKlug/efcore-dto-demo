using EFCore.Domain.PeopleManagement.Data;
using EFCore.Domain.Infrastructure;
using EFCore.Domain.PeopleManagement.DTOs;
using EFCore.Domain.PeopleManagement.Exceptions;
using EFCore.Domain.Data;

namespace EFCore.Domain.PeopleManagement;

public class PersonService
{
    private readonly DemoContext context;

    public PersonService(DemoContext context)
    {
        this.context = context;
    }

    public async Task<ServiceResult<PersonDTO>> GetPersonById(int id)
    {
        var person = await context.PersonWithId(id, asNoTracking: true);

        return person != null ?
                    ServiceResult.Success(person.ToModel()) :
                    ServiceResult.Fail<PersonDTO>(new PersonNotFoundException());
    }

    public async Task<ServiceResult<AddressDTO[]>> GetPersonsAddresses(int personId)
    {
        var person = await context.PersonWithId(personId, asNoTracking: true);

        return person != null ?
                    ServiceResult.Success(person.GetAllAddresses().Select(x => x.ToModel()!).ToArray()) :
                    ServiceResult.Fail<AddressDTO[]>(new PersonNotFoundException());
    }

    public async Task<ServiceResult<AddressDTO>> SetPersonsAddress(int personId,
                                                                   AddressType type,
                                                                   string addressLine1,
                                                                   string addressLine2,
                                                                   string postalCode,
                                                                   string city,
                                                                   string country)
    {
        var person = await context.PersonWithId(personId);
        if (person == null)
        {
            return ServiceResult.Fail<AddressDTO>(new PersonNotFoundException());
        }

        Address newAddress;
        try
        {
            if (type == AddressType.Delivery)
            {
                newAddress = person.SetDeliveryAddress(addressLine1, addressLine2, postalCode, city, country);
            }
            else
            {
                newAddress = person.SetInvoiceAddress(addressLine1, addressLine2, postalCode, city, country);
            }

            var x = await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail<AddressDTO>(ex);
        }

        return ServiceResult.Success(newAddress.ToModel()!);
    }
}

public enum AddressType
{
    Delivery,
    Invoice
}