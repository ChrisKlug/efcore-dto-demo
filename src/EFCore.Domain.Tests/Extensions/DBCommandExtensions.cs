using System.Data.Common;

namespace EFCore.Domain.Tests.Extensions;

public static class DBCommandExtensions
{
    public static async Task AddPerson(this DbCommand cmd, int id, string firstName, string lastName, 
                                        (string Type, string AddressLine1, string AddressLine2, string PostalCode, string City, string Country, bool IsCurrent)[]? addresses = null)
    {
        cmd.CommandText = "SET IDENTITY_INSERT People ON; " +
                        $"INSERT INTO People (Id, FirstName, LastName) VALUES ({id}, '{firstName}', '{lastName}')" +
                        "SET IDENTITY_INSERT People OFF;";
        await cmd.ExecuteNonQueryAsync();

        if (addresses != null && addresses.Length > 0)
        {
            cmd.CommandText = $"INSERT INTO Addresses (Type, AddressLine1, AddressLine2, PostalCode, City, Country, IsCurrent, PersonId) VALUES ";
            foreach (var address in addresses)
            {
                cmd.CommandText += $"('{address.Type}', '{address.AddressLine1}', '{address.AddressLine2}', '{address.PostalCode}', '{address.City}', '{address.Country}', '{(address.IsCurrent ? 1 : 0)}', {id}), \r\n";
            }
            cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.LastIndexOf(","));
            await cmd.ExecuteNonQueryAsync();
        }
    }

    public static async Task AddVehicle(this DbCommand cmd, int id, string vin, (int Id, string FirstName, string LastName, DateTime From, DateTime? To)[]? owners = null)
    {
        cmd.CommandText = "SET IDENTITY_INSERT Vehicles ON; " +
                        $"INSERT INTO Vehicles (Id, VIN) VALUES ({id}, '{vin}')" +
                        "SET IDENTITY_INSERT Vehicles OFF;";
        await cmd.ExecuteNonQueryAsync();

        if (owners != null && owners.Length > 0)
        {
            foreach (var owner in owners)
            {
                cmd.CommandText = "SET IDENTITY_INSERT People ON; " +
                                  $"INSERT INTO People (Id, FirstName, LastName) VALUES ({owner.Id},'{owner.FirstName}','{owner.LastName}');" +
                                  "SET IDENTITY_INSERT Vehicles OFF;";
                await cmd.ExecuteNonQueryAsync();
                cmd.CommandText = $"INSERT INTO VehicleOwners (VehicleId, PersonId, [From], [To]) VALUES ({id},{owner.Id},'{owner.From}',{(owner.To == null ? "null" : $"'{owner.To}'")})";
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}
