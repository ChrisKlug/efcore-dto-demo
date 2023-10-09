using EFCore.Domain.VehicleManagement.Exceptions;
namespace EFCore.Domain.VehicleManagement;

public class Vehicle
{
    private List<Owner> owners = new List<Owner>();

    private Vehicle() { }
    private Vehicle(VIN vin)
    {
        VIN = vin;
    }

    public static Vehicle Create(VIN vin, Person owner)
    {
        var vehicle = new Vehicle(vin);

        vehicle.owners.Add(Owner.Create(owner));

        return vehicle;
    }

    public Owner SetOwner(Person newOwner)
    {
        if (CurrentOwner != null)
        {
            if (newOwner.Id == CurrentOwner.Id)
            {
                throw new DuplicateOwnerException();
            }
            CurrentOwner!.EndOwnership();
        }

        var owner = Owner.Create(newOwner);
        owners.Add(owner);

        return owner;
    }

    public VIN VIN { get; private set; }

    public Owner? CurrentOwner => owners.FirstOrDefault(x => x.To == null);
    public Owner[] PreviousOwners => owners.Where(x => x.To != null).ToArray();
}
