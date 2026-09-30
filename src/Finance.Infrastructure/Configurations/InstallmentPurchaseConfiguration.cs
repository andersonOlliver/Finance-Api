using Finance.Domain.Categories;
using Finance.Domain.CreditCards;
using Finance.Domain.Installments;
using Finance.Domain.Transactions;
using Finance.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations;

internal sealed class InstallmentPurchaseConfiguration : IEntityTypeConfiguration<InstallmentPurchase>
{
    public void Configure(EntityTypeBuilder<InstallmentPurchase> builder)
    {
        builder.ToTable("installment_purchases");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .HasMaxLength(200)
            .HasConversion(name => name.Value, value => Title.Create(value).Value);

        builder.OwnsOne(p => p.TotalAmount, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
                .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .IsRequired(true);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .IsRequired(true);

        builder.HasOne<CreditCard>()
            .WithMany()
            .HasForeignKey(p => p.CreditCardId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(true);
    }
}
