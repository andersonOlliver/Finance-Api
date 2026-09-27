using Finance.Application.Categories.SearchCategories;
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
}
