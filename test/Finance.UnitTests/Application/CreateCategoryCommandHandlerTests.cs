using Finance.Application.Categories.CreateCategory;
using Finance.Application.Exceptions;
using Finance.Domain.Categories;
using FluentAssertions;
using NSubstitute;

namespace Finance.UnitTests.Application;

public class CreateCategoryCommandHandlerTests
{
    private readonly ApplicationTestHarness _harness = new();

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddCategoryOwnedByCurrentUser()
    {
        var userId = Guid.NewGuid();
        _harness.UserContext.UserId.Returns(userId);

        var command = new CreateCategoryCommand("Lazer", CategoryType.Expense, "F2994A", "wallet");

        var result = await _harness.Sender.Send(command);

        result.IsSuccess.Should().BeTrue();

        _harness.CategoryRepository.Received(1).Add(Arg.Is<Category>(c =>
            c.Id == result.Value &&
            c.Name.Value == "Lazer" &&
            c.Type == CategoryType.Expense &&
            c.UserId == userId));

        await _harness.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldGenerateAVersion7Id()
    {
        var command = new CreateCategoryCommand("Lazer", CategoryType.Expense, "F2994A", "wallet");

        var result = await _harness.Sender.Send(command);

        result.Value.ToString()[14].Should().Be('7');
    }

    [Fact]
    public async Task Handle_WithEmptyName_ShouldThrowValidationException()
    {
        var command = new CreateCategoryCommand(string.Empty, CategoryType.Expense, "F2994A", "wallet");

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
        _harness.CategoryRepository.DidNotReceive().Add(Arg.Any<Category>());
    }

    [Fact]
    public async Task Handle_WithInvalidType_ShouldThrowValidationException()
    {
        var command = new CreateCategoryCommand("Lazer", (CategoryType)999, "F2994A", "wallet");

        var act = () => _harness.Sender.Send(command);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
