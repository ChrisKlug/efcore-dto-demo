using EFCore.DTO.Data.Models;

namespace EFCore.DTO.Data;

public static class SeedData
{
    public static void AddSeedData(this VehicleRegistryContext context)
    {
        var vehicleCount = context.Vehicles.Count();
        if (vehicleCount > 0)
        {
            return;
        }

        var address1 = context.Addresses.Add(new Address
        {
            Type = AddressType.Delivery,
            AddressLine1 = "Street 1",
            PostalCode = "12345",
            City = "The City",
            Country = "Sweden",
            IsCurrent = true
        }).Entity;

        var address2 = context.Addresses.Add(new Address
        {
            Type = AddressType.Invoice,
            AddressLine1 = "Other Street 1",
            PostalCode = "98765",
            City = "Yoville",
            Country = "Sweden",
            IsCurrent = true
        }).Entity;

        var john = context.People.Add(new Person {
            FirstName = "John",
            LastName = "Doe",
            Addresses = new[] { address1, address2 }
        }).Entity;

        var address3 = context.Addresses.Add(new Address
        {
            Type = AddressType.Delivery,
            AddressLine1 = "Street 2",
            PostalCode = "12345",
            City = "The City",
            Country = "Sweden",
            IsCurrent = true
        }).Entity;

        var address4 = context.Addresses.Add(new Address
        {
            Type = AddressType.Invoice,
            AddressLine1 = "Other Street 2",
            PostalCode = "98765",
            City = "Yoville",
            Country = "Sweden",
            IsCurrent = true
        }).Entity;

        var jane = context.People.Add(new Person {
            FirstName = "Jane",
            LastName = "Doe",
            Addresses = new[] { address3, address4 }
        }).Entity;

        var vehicle1 = context.Vehicles.Add(new Vehicle { 
            VIN = "VIN1"
        }).Entity;

        context.Vehicles.Add(new Vehicle {
            VIN = "VIN2"
        });

        context.VehicleOwners.Add(new VehicleOwner {
            Person = john,
            Vehicle = vehicle1,
            From = DateTime.Today.AddYears(-1)
        });

        context.SaveChanges();
    }
}
