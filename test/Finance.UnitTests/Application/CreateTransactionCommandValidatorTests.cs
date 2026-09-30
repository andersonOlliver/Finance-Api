using Finance.Application;
using Finance.Application.Transactions.CreateTransaction;
using FluentValidation;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.UnitTests.Application;

public class CreateTransactionCommandValidatorTests
{
    private readonly IValidator<CreateTransactionCommand> _validator;

    public CreateTransactionCommandValidatorTests()
    {
        var services = new ServiceCollection();
        services.AddApplication();
        _validator = services.BuildServiceProvider().GetRequiredService<IValidator<CreateTransactionCommand>>();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        var command = new CreateTransactionCommand("Mercado", 150m, "USD", "Compras", Guid.NewGuid(), null, null, DateTime.UtcNow);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyTitle_ShouldHaveErrorForTitle()
    {
        var command = new CreateTransactionCommand(string.Empty, 150m, "USD", null, Guid.NewGuid(), null, null, DateTime.UtcNow);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Fact]
    public void Validate_WithNonPositiveAmount_ShouldHaveErrorForAmount()
    {
        var command = new CreateTransactionCommand("Mercado", 0m, "USD", null, Guid.NewGuid(), null, null, DateTime.UtcNow);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.Amount);
    }

    [Fact]
    public void Validate_WithUnknownCurrencyCode_ShouldHaveErrorForCurrencyCode()
    {
        var command = new CreateTransactionCommand("Mercado", 150m, "XYZ", null, Guid.NewGuid(), null, null, DateTime.UtcNow);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CurrencyCode);
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_ShouldHaveErrorForCategoryId()
    {
        var command = new CreateTransactionCommand("Mercado", 150m, "USD", null, Guid.Empty, null, null, DateTime.UtcNow);

        var result = _validator.TestValidate(command);

        result.ShouldHaveValidationErrorFor(c => c.CategoryId);
    }
}
