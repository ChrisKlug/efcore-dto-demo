namespace EFCore.Domain.VehicleManagement;

public class Owner
{
    private Person person;

    private Owner() { }
    private Owner(Person person)
    {
        this.person = person;
    }

    public static Owner Create(Person owner) => new Owner(owner);

    public void EndOwnership() => To = DateTime.Today;

    public int Id => person.Id;
    public string FirstName => person.FirstName;
    public string LastName => person.LastName;
    public DateTime From { get; private set; } = DateTime.Today;
    public DateTime? To { get; private set; }
}