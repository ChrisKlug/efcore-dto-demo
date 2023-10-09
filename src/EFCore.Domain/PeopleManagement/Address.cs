namespace EFCore.Domain.PeopleManagement;

public abstract class Address
{
    protected Address() { }

    public static T Create<T>(string addressLine1, string? addressLine2,
                              string postalCode, string city, string country,
                              bool isCurrent) where T : Address, new()
    {
        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentNullException(nameof(addressLine1));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentNullException(nameof(postalCode));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentNullException(nameof(city));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentNullException(nameof(country));

        return new T
        {
            AddressLine1 = addressLine1,
            AddressLine2 = addressLine2,
            PostalCode = postalCode,
            City = city,
            Country = country,
            IsCurrent = isCurrent
        };
    }

    public void Disable() => IsCurrent = false;

    public string AddressLine1 { get; private set; } = string.Empty;
    public string? AddressLine2 { get; private set; }
    public string PostalCode { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public bool IsCurrent { get; private set; } = false;
}
public class DeliveryAddress : Address { }

public class InvoiceAddress : Address { }
