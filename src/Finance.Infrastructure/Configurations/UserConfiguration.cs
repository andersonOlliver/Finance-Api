using Finance.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Finance.Infrastructure.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .HasMaxLength(200)
            .HasConversion(name => name.Value, value => new FirstName(value));

        builder.Property(u => u.LastName)
            .HasMaxLength(200)
            .HasConversion(name => name.Value, value => new LastName(value));

        builder.Property(u => u.Email)
            .HasMaxLength(400)
            .HasConversion(email => email.Value, value => new Email(value));

        builder.HasIndex(user => user.Email).IsUnique();
    }
}
