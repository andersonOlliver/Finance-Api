using Finance.Domain.Shared;
using Finance.Domain.Users;
using Finance.Domain.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations;

internal sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Nickname)
            .HasMaxLength(100)
            .HasConversion(name => name.Value, value => new Name(value));

        builder.Property(v => v.Brand)
            .HasMaxLength(100);

        builder.Property(v => v.Model)
            .HasMaxLength(100);

        builder.Property(v => v.Color)
            .HasMaxLength(50);

        builder.Property(v => v.LicensePlate)
            .HasMaxLength(10);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(v => v.UserId)
            .IsRequired(true);
    }
}
