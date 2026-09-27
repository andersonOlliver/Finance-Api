using Finance.Application;
using Finance.Application.Abstractions.Authentication;
using Finance.Application.Abstractions.Clock;
using Finance.Domain.Abstracts;
using Finance.Domain.Categories;
using Finance.Domain.Payments;
using Finance.Domain.Transactions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Finance.UnitTests.Application;

internal sealed class ApplicationTestHarness
{
    public ITransactionRepository TransactionRepository { get; } = Substitute.For<ITransactionRepository>();
    public ICategoryRepository CategoryRepository { get; } = Substitute.For<ICategoryRepository>();
    public IPaymentRepository PaymentRepository { get; } = Substitute.For<IPaymentRepository>();
    public IUserContext UserContext { get; } = Substitute.For<IUserContext>();
    public IDateTimeProvider DateTimeProvider { get; } = Substitute.For<IDateTimeProvider>();
    public IUnitOfWork UnitOfWork { get; } = Substitute.For<IUnitOfWork>();
    public ISender Sender { get; }

    public ApplicationTestHarness()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddApplication();

        services.AddSingleton(TransactionRepository);
        services.AddSingleton(CategoryRepository);
        services.AddSingleton(PaymentRepository);
        services.AddSingleton(UserContext);
        services.AddSingleton(DateTimeProvider);
        services.AddSingleton(UnitOfWork);

        Sender = services.BuildServiceProvider().GetRequiredService<ISender>();
    }
}
