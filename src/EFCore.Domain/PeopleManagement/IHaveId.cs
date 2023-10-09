namespace EFCore.Domain.PeopleManagement;

public interface IHaveId<T>
{
    T Id { get; }
}