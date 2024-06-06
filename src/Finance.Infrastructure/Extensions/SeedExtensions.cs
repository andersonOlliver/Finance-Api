using Finance.Domain.Categories;
using Finance.Domain.Payments;
using Finance.Domain.Shared;
using Finance.Domain.Users;

namespace Finance.Infrastructure.Extensions;

public static class SeedExtensions
{
    public static void SeedDatabase(this ApplicationDbContext dbContext)
    {
        dbContext
            .RegisterTestUser()
            .PopulateCategory()
            .PopulatePayment()
            .SaveChangesWithoutEventsAsync()
            .Wait();
    }

    private static ApplicationDbContext RegisterTestUser(this ApplicationDbContext dbContext)
    {
        if (!dbContext.Set<User>().Any())
        {
            dbContext.Set<User>()
                .Add(User.Create(
                    new FirstName("Anderson"),
                    new LastName("Olliver"),
                    new Email("pla.olliver@gmail.com"),
                    DateTime.UtcNow));
        }
        return dbContext;
    }

    private static ApplicationDbContext PopulateCategory(this ApplicationDbContext dbContext)
    {
        if (!dbContext.Set<Category>().Any())
        {
            dbContext.Set<Category>().AddRange(
                new List<Category>()
                {
                    Category.Create(Guid.NewGuid(), new Name("Salário"), CategoryType.Receive, Color.Green, Icon.Money, DateTime.UtcNow),
                    Category.Create(Guid.NewGuid(), new Name("Alimentação"), CategoryType.Expense, Color.Orange, Icon.Restaurant, DateTime.UtcNow),
                    Category.Create(Guid.NewGuid(), new Name("Investimento"), CategoryType.Expense, Color.GreenLight, Icon.Wallet, DateTime.UtcNow)
                }
            );
        }
        return dbContext;
    }

    private static ApplicationDbContext PopulatePayment(this ApplicationDbContext dbContext)
    {
        if (!dbContext.Set<Payment>().Any())
        {
            dbContext.Set<Payment>().AddRange(new List<Payment>() {
                Payment.Create(Guid.NewGuid(), new Name("Dinheiro"), PaymentType.Money, null, DateTime.UtcNow),
                Payment.Create(Guid.NewGuid(), new Name("Débito"), PaymentType.Debit, null, DateTime.UtcNow),
                Payment.Create(Guid.NewGuid(), new Name("Crédito"), PaymentType.CashCredit, null, DateTime.UtcNow),
                Payment.Create(Guid.NewGuid(), new Name("Crédito Parcelado"), PaymentType.InstallmentCredit, null, DateTime.UtcNow),
        });
        }
        return dbContext;
    }
}
