using Finance.Application.Payments.SearchPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Payments;

[Authorize]
[ApiController]
[Route("api/payments")]
public class PaymentsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchPayments(CancellationToken cancellationToken)
    {
        var query = new SearchPaymentsQuery();
        var result = await sender.Send(query, cancellationToken);
        return Ok(result.Value);
    }
}
