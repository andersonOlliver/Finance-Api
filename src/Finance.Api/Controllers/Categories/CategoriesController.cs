using Finance.Application.Categories.CreateCategory;
using Finance.Application.Categories.DeleteCategory;
using Finance.Application.Categories.GetCategoryById;
using Finance.Application.Categories.SearchCategories;
using Finance.Application.Categories.UpdateCategory;
using Finance.Domain.Abstracts;
using Finance.Domain.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Categories;

[Authorize]
[ApiController]
[Route("api/categories")]
public class CategoriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchCategories(CancellationToken cancellationToken)
    {
        var query = new SearchCategoryQuery();
        var result = await sender.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ToProblemResult(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name, request.Type, request.Color, request.Icon);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCategoryById), new { id = result.Value }, result.Value)
            : ToProblemResult(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCategory(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCategoryCommand(id, request.Name, request.Type, request.Color, request.Icon);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    private IActionResult ToProblemResult(Error error)
    {
        return error == CategoryErrors.NotFound
            ? NotFound(error)
            : BadRequest(error);
    }
}
