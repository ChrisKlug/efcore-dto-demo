namespace EFCore.Domain.PeopleManagement;

public class Person
{
    private int id;
    private List<Address> addresses = new List<Address>();
    private Person() { }

    public static Person Create(string firstName, string lastName)
        => new Person { FirstName = firstName, LastName = lastName };

    public Address[] GetAllAddresses() => addresses.ToArray();
    public Address SetDeliveryAddress(string addressLine1,
                                     string addressLine2,
                                     string postalCode,
                                     string city,
                                     string country)
    {
        return SetAddress<DeliveryAddress>(addressLine1, addressLine2, postalCode, city, country);
    }
    public Address SetInvoiceAddress(string addressLine1,
                                     string addressLine2,
                                     string postalCode,
                                     string city,
                                     string country)
    {
        return SetAddress<InvoiceAddress>(addressLine1, addressLine2, postalCode, city, country);
    }

    private T SetAddress<T>(string addressLine1,
                               string addressLine2,
                               string postalCode,
                               string city,
                               string country) where T : Address, new()
    {
        addresses.OfType<T>().FirstOrDefault(x => x.IsCurrent)?.Disable();

        var newAddress = Address.Create<T>(addressLine1, addressLine2, postalCode, city, country, true);

        addresses.Add(newAddress);

        var currentAddresses = addresses.OfType<T>().ToArray();
        if (currentAddresses.Length > 3)
        {
            for (var i = 0; i < currentAddresses.Length - 3; i++)
            {
                addresses.Remove(currentAddresses[i]);
            }
        }

        return newAddress;
    }

    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Address? DeliveryAddress
        => addresses.OfType<DeliveryAddress>().FirstOrDefault(x => x.IsCurrent);
    public Address? InvoiceAddress
        => addresses.OfType<InvoiceAddress>().FirstOrDefault(x => x.IsCurrent);
}
