using EFCore.DTO.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.DTO.Data;

public class VehicleRegistryContext : DbContext
{
    public VehicleRegistryContext() { }

    public VehicleRegistryContext(DbContextOptions<VehicleRegistryContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<VehicleOwner> VehicleOwners { get; set; }
    public DbSet<Address> Addresses { get; set; }
}
