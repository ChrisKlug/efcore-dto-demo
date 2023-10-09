using EFCore.DTO.Data;
using EFCore.DTO.Wrapper.Exceptions;
using EFCore.DTO.Wrapper.Extensions;
using EFCore.DTO.Wrapper.Models;
using EFCore.DTO.Wrapper.Entities;

namespace EFCore.DTO.Wrapper;

public class PersonService
{
    private readonly VehicleRegistryContext context;

    public PersonService(VehicleRegistryContext context)
    {
        this.context = context;
    }

    public async Task<ServiceResult<PersonDTO>> GetPersonById(int id)
    {
        var person = await context.PersonWithId(id, asNoTracking: true);

        return person != null ?
                    ServiceResult.Success(PersonDTO.Create(person)) :
                    ServiceResult.Fail<PersonDTO>(new PersonNotFoundException());
    }

    public async Task<ServiceResult<AddressDTO[]>> GetPersonsAddresses(int personId)
    {
        var person = await context.PersonWithId(personId, asNoTracking: true);

        return person != null ?
                    ServiceResult.Success(person.GetAllAddresses().Select(x => AddressDTO.Create(x)!).ToArray()) :
                    ServiceResult.Fail<AddressDTO[]>(new PersonNotFoundException());
    }

    public async Task<ServiceResult<AddressDTO>> SetPersonsAddress(int personId, 
                                                                   Data.Models.AddressType type, 
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
            if (type == Data.Models.AddressType.Delivery)
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

        return ServiceResult.Success(AddressDTO.Create(newAddress)!);
    }
}
