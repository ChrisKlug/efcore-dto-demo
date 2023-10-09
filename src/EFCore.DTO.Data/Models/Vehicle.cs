using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.DTO.Data.Models;

public class Vehicle
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public string VIN { get; set; } = string.Empty;
    public ICollection<VehicleOwner> Owners { get; set; }
}
