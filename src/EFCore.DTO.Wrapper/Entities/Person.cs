namespace EFCore.DTO.Wrapper.Entities;

public class Person
{
    private readonly Data.Models.Person person;
    private Address? deliveryAddress;
    private Address? invoiceAddress;

    private Person(Data.Models.Person person)
    {
        this.person = person;

        deliveryAddress = Address.Create(this.person.Addresses?
                                                    .Where(x => x.IsCurrent && x.Type == Data.Models.AddressType.Delivery)
                                                    .FirstOrDefault());
        invoiceAddress = Address.Create(this.person.Addresses?
                                                    .Where(x => x.IsCurrent && x.Type == Data.Models.AddressType.Invoice)
                                                    .FirstOrDefault());
    }

    public static Person Create(Data.Models.Person person) => new Person(person);

    public Address[] GetAllAddresses() => person.Addresses.Select(x => Address.Create(x)!).ToArray();
    public Address SetDeliveryAddress(string addressLine1,
                                     string addressLine2,
                                     string postalCode,
                                     string city,
                                     string country)
    {
        deliveryAddress = SetAddress(Data.Models.AddressType.Delivery, addressLine1, addressLine2, postalCode, city, country);
        return deliveryAddress;
    }
    public Address SetInvoiceAddress(string addressLine1,
                                     string addressLine2,
                                     string postalCode,
                                     string city,
                                     string country)
    {
        invoiceAddress = SetAddress(Data.Models.AddressType.Invoice, addressLine1, addressLine2, postalCode, city, country);
        return invoiceAddress;
    }

    private Address SetAddress(Data.Models.AddressType type,
                               string addressLine1,
                               string addressLine2,
                               string postalCode,
                               string city,
                               string country)
    {
        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentNullException(nameof(addressLine1));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentNullException(nameof(postalCode));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentNullException(nameof(city));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentNullException(nameof(country));

        var existingAddress = person.Addresses.FirstOrDefault(x => x.Type == type && x.IsCurrent);
        if (existingAddress != null)
        {
            existingAddress.IsCurrent = false;
        }

        var newAddress = new Data.Models.Address
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
            for (var i = 0; i < addresses.Length - 3; i++)
            {
                person.Addresses.Remove(addresses[i]);
            }
        }

        return Address.Create(newAddress)!;
    }

    public int Id => person.Id;
    public string FirstName => person.FirstName;
    public string LastName => person.LastName;
    public Address? DeliveryAddress => deliveryAddress;
    public Address? InvoiceAddress => invoiceAddress;

    public static explicit operator Data.Models.Person(Person person) => person.person;
}
