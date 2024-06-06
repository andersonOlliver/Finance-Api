using Finance.Domain.Categories;
using Finance.Domain.Payments;
using Finance.Domain.Transactions;
using Finance.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations;

internal sealed class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(ct => ct.Id);

        builder.Property(p => p.Title)
            .HasMaxLength(200)
           .HasConversion(name => name.Value, value => Title.Create(value).Value);

        builder.OwnsOne(b => b.Value, pricebuilder =>
        {
            pricebuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });

        builder.Property(p => p.Description)
            .HasMaxLength(200)
            .HasConversion(name => name.Value, value => new Description(value))
            .IsRequired(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .IsRequired(false);
        
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .IsRequired(true);
        
        builder.HasOne<Payment>()
            .WithMany()
            .HasForeignKey(b => b.PaymentId)
            .IsRequired(false);

    }
}
