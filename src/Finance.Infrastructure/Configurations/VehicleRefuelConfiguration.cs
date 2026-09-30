using Finance.Domain.Transactions;
using Finance.Domain.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations;

internal sealed class VehicleRefuelConfiguration : IEntityTypeConfiguration<VehicleRefuel>
{
    public void Configure(EntityTypeBuilder<VehicleRefuel> builder)
    {
        builder.ToTable("vehicle_refuels");

        builder.HasKey(r => r.Id);

        builder.HasOne<Vehicle>()
            .WithMany()
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasOne<Transaction>()
            .WithMany()
            .HasForeignKey(r => r.TransactionId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);
    }
}
