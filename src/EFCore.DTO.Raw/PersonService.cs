using EFCore.DTO.Data;
using EFCore.DTO.Data.Models;
using EFCore.DTO.Raw.Exceptions;
using EFCore.DTO.Raw.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.DTO.Raw;

public class PersonService
{
    private readonly VehicleRegistryContext context;

    public PersonService(VehicleRegistryContext context)
    {
        this.context = context;
    }

    public async Task<ServiceResult<PersonDTO>> GetPersonById(int id)
    {
        var person = await context.People
                                    .Include(x => x.Addresses)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.Id == id);

        return person != null ?
                    ServiceResult.Success(PersonDTO.Create(person, person.Addresses.Where(x => x.IsCurrent).ToArray())) :
                    ServiceResult.Fail<PersonDTO>(new PersonNotFoundException());
    }

    public async Task<ServiceResult<AddressDTO[]>> GetPersonsAddresses(int personId)
    {
        var person = await context.People
                                    .Include(x => x.Addresses)
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(x => x.Id == personId);

        return person != null ?
                    ServiceResult.Success(person.Addresses.Select(AddressDTO.Create).ToArray()) :
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
        if (string.IsNullOrWhiteSpace(addressLine1))
            return ServiceResult.Fail<AddressDTO>(new ArgumentNullException(nameof(addressLine1)));

        if (string.IsNullOrWhiteSpace(postalCode))
            return ServiceResult.Fail<AddressDTO>(new ArgumentNullException(nameof(postalCode)));

        if (string.IsNullOrWhiteSpace(city))
            return ServiceResult.Fail<AddressDTO>(new ArgumentNullException(nameof(city)));

        if (string.IsNullOrWhiteSpace(country))
            return ServiceResult.Fail<AddressDTO>(new ArgumentNullException(nameof(country)));

        var person = await context.People
                                    .Include(x => x.Addresses)
                                    .SingleOrDefaultAsync(x => x.Id == personId);

        if (person == null) 
        {
            return ServiceResult.Fail<AddressDTO>(new PersonNotFoundException());
        }

        var existingAddress = person.Addresses.FirstOrDefault(x => x.Type == type && x.IsCurrent);
        if (existingAddress != null)
        {
            existingAddress.IsCurrent = false;
        }

        var newAddress = new Address
        {
            Type = type,
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            PostalCode = postalCode,
            City = city,
            Country = country,
            IsCurrent = true
        };

        person.Addresses.Add(newAddress);

        var addresses = person.Addresses.Where(x => x.Type == type).ToArray();
        if (addresses.Length > 3)
        {
            for ( var i = 0; i < addresses.Length - 3; i++)
            {
                person.Addresses.Remove(addresses[i]);
            }
        }

        try
        {
            var x = await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ServiceResult.Fail<AddressDTO>(ex);
        }

        return ServiceResult.Success(AddressDTO.Create(newAddress));
    }
}
