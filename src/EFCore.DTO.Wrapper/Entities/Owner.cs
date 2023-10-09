namespace EFCore.DTO.Wrapper.Entities;

public class Owner
{
    private readonly Data.Models.VehicleOwner owner;

    private Owner(Data.Models.VehicleOwner owner)
    {
        this.owner = owner;
    }

    public void EndOwnership()
    {
        owner.To = DateTime.Today;
    }

    public static Owner Create(Data.Models.VehicleOwner owner) => new Owner(owner);

    public int Id => owner.Person.Id;
    public string FirstName => owner.Person.FirstName;
    public string LastName => owner.Person.LastName;
    public DateTime From => owner.From;
    public DateTime? To => owner.To;
    public bool IsCurrentOwner => owner.To == null;
}
