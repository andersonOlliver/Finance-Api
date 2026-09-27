using Finance.Application.Categories.UpdateCategory;
using Finance.Domain.Categories;
using Finance.Domain.Shared;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class UpdateCategoryCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    private static Category CreateExistingCategory(Guid? userId) =>
        Category.Create(Guid.NewGuid(), new Name("Lazer"), CategoryType.Expense, Color.Orange, Icon.Wallet, DateTime.UtcNow, userId);

    [Fact]
    public async Task Handle_WithOwnedCategory_ShouldUpdateItAndSucceed()
    {
        var userId = Guid.NewGuid();
        var category = CreateExistingCategory(userId);

        _harness.UserContext.UserId.Returns(userId);
        _harness.CategoryRepository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var command = new UpdateCategoryCommand(category.Id, "Lazer e hobbies", CategoryType.Expense, "219653", "home");

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();
        category.Name.Value.Should().Be("Lazer e hobbies");
        category.Icon.Value.Should().Be("home");

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenCategoryIsADefaultCategory_ShouldReturnNotFound()
    {
        var category = CreateExistingCategory(null);

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.CategoryRepository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var command = new UpdateCategoryCommand(category.Id, "Lazer e hobbies", CategoryType.Expense, "219653", "home");

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenCategoryBelongsToAnotherUser_ShouldReturnNotFound()
    {
        var category = CreateExistingCategory(Guid.NewGuid());

        _harness.UserContext.UserId.Returns(Guid.NewGuid());
        _harness.CategoryRepository.GetByIdAsync(category.Id, Arg.Any<CancellationToken>()).Returns(category);

        var command = new UpdateCategoryCommand(category.Id, "Lazer e hobbies", CategoryType.Expense, "219653", "home");

        var result = await _harness.Sender.Send(command);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(CategoryErrors.NotFound);
    }
}
