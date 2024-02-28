using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Domain.PeopleManagement.Data;

public class PersonTypeConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");
        builder.Property<int>("id");

        builder.HasOne<VehicleManagement.Person>().WithOne().HasForeignKey<Person>("id");

        builder.HasMany<Address>("addresses").WithOne().HasForeignKey("PersonId").IsRequired();
        builder.Navigation("addresses").AutoInclude();

        builder.ComplexProperty(x => x.Name, x =>
        {
            x.Property(y => y.FirstName).HasColumnName("FirstName");
            x.Property(y => y.LastName).HasColumnName("LastName");
        });

        builder.Ignore(x => x.DeliveryAddress);
        builder.Ignore(x => x.InvoiceAddress);

        builder.HasKey("id");
    }
}
