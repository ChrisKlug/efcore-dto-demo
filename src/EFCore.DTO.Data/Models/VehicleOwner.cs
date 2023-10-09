using Microsoft.EntityFrameworkCore;

namespace EFCore.DTO.Data.Models;

[PrimaryKey("VehicleId", "PersonId")]
public class VehicleOwner
{
    public int VehicleId { get; set; }
    public Vehicle Vehicle { get; set; }

    public int PersonId { get; set; }
    public Person Person { get; set; }

    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}
