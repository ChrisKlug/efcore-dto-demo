namespace EFCore.DTO.Wrapper.Entities;

public record Name
{
    public Name(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentNullException(nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentNullException(nameof(lastName));
        
        FirstName = firstName;
        LastName = lastName;
    }
    
    public string FirstName { get; }
    public string LastName { get; }
}