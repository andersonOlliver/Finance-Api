using Finance.Domain.Payments;
using Finance.Domain.Shared;
using Finance.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        
        builder.HasKey(ct => ct.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(200)
           .HasConversion(name => name.Value, value => new Name(value));
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .IsRequired(false);

    }
}
