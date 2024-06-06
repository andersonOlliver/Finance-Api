using Finance.Domain.Categories;
using Finance.Domain.Shared;
using Finance.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        
        builder.HasKey(ct => ct.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(200)
           .HasConversion(name => name.Value, value => new Name(value));
        
        builder.Property(p => p.Color)
            .HasMaxLength(8)
           .HasConversion(name => name.Value, value => new Color(value));
        
        builder.Property(p => p.Icon)
            .HasMaxLength(20)
           .HasConversion(name => name.Value, value => new Icon(value));

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .IsRequired(false);

    }
}
