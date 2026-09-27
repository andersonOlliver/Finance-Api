using Finance.Application.Categories.DeleteCategory;
using Finance.Domain.Categories;
using Finance.Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class DeleteCategoryCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Category CreateExistingCategory(Guid? userId) =>
        Category.Create(Guid.NewGuid(), new Name("Lazer"), CategoryType.Expense, Color.Orange, Icon.Wallet, DateTime.UtcNow, userId);

    [Fact]
    public async Task Handle_WithOwnedCategory_ShouldRemoveItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var category = CreateExistingCategory(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.CategoryRepository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var result = await _harness.Sender.Send(new DeleteCategoryCommand(category.Id));

        result.IsSuccess.Should().BeTrue();
        _harness.CategoryRepository.Received(1).Remove(category);
    }

    [Fact]
    public async Task Handle_WhenCategoryIsADefaultCategory_ShouldReturnNotFoundAndNotRemove()
    {
        var category = CreateExistingCategory(null);

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.CategoryRepository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var result = await _harness.Sender.Send(new DeleteCategoryCommand(category.Id));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.NotFound);
        _harness.CategoryRepository.DidNotReceive().Remove(category);
    }
}
