using Finance.Domain.CreditCards;
using Finance.Domain.Shared;
using Finance.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations;

internal sealed class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
{
    public void Configure(EntityTypeBuilder<CreditCard> builder)
    {
        builder.ToTable("credit_cards");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Nickname)
            .HasMaxLength(100)
            .HasConversion(name => name.Value, value => new Name(value));

        builder.Property(c => c.Brand)
            .HasMaxLength(50);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .IsRequired(true);
    }
}
