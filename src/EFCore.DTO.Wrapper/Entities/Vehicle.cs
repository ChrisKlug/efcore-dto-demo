using EFCore.DTO.Wrapper.Exceptions;
using System.Text.RegularExpressions;

namespace EFCore.DTO.Wrapper.Entities;

public class Vehicle
{
    private static Regex VinRegex = new Regex("[A-HJ-NPR-Z0-9]{17}");
    private readonly Data.Models.Vehicle vehicle;

    private Vehicle(Data.Models.Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }

    public Owner SetOwner(Person newOwner)
    {
        if (CurrentOwner != null)
        {
            if (newOwner.Id == CurrentOwner?.Id)
            {
                throw new DuplicateOwnerException();
            }
            CurrentOwner!.EndOwnership();
        }

        var pv = new Data.Models.VehicleOwner
        {
            Person = (Data.Models.Person)newOwner,
            Vehicle = vehicle,
            From = DateTime.Today
        };

        vehicle.Owners.Add(pv);

        return Owner.Create(pv);
    }

    public static Vehicle Create(string vin, Person owner)
    {
        if (!VinRegex.IsMatch(vin))
        {
            throw new InvalidVinException();
        }

        var vehicle = new Data.Models.Vehicle
        {
            VIN = vin
        };

        vehicle.Owners = new List<Data.Models.VehicleOwner> {
                new Data.Models.VehicleOwner {
                    Vehicle = vehicle,
                    Person = (Data.Models.Person)owner,
                    From = DateTime.Today
                }
            };

        return new Vehicle(vehicle);
    }
    public static Vehicle Create(Data.Models.Vehicle vehicle) => new Vehicle(vehicle);

    public string VIN => vehicle.VIN;
    public Owner? CurrentOwner => vehicle.Owners.Where(x => x.To == null).Select(Owner.Create).FirstOrDefault();
    public Owner[] PreviousOwners => vehicle.Owners.Where(x => x.To != null).Select(Owner.Create).ToArray();

    public static explicit operator Data.Models.Vehicle(Vehicle vehicle) => vehicle.vehicle;
}
