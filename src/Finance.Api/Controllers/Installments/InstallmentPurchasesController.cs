using Finance.Application.Installments.CreateInstallmentPurchase;
using Finance.Application.Installments.DeleteInstallmentPurchase;
using Finance.Application.Installments.GetInstallmentPurchaseById;
using Finance.Application.Installments.SearchInstallmentPurchases;
using Finance.Domain.Abstracts;
using Finance.Domain.Installments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Installments;

[Authorize]
[ApiController]
[Route("api/installment-purchases")]
public class InstallmentPurchasesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchInstallmentPurchases(
        [FromQuery] DateTime? month,
        [FromQuery] Guid? creditCardId,
        CancellationToken cancellationToken)
    {
        var query = new SearchInstallmentPurchasesQuery(month, creditCardId);
        var result = await sender.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetInstallmentPurchaseById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetInstallmentPurchaseByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ToProblemResult(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInstallmentPurchase(CreateInstallmentPurchaseRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateInstallmentPurchaseCommand(
            request.Title,
            request.TotalAmount,
            request.CurrencyCode,
            request.InstallmentCount,
            request.CategoryId,
            request.CreditCardId,
            request.Description,
            request.FirstInstallmentReleasedOnUtc);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetInstallmentPurchaseById), new { id = result.Value.Id }, result.Value)
            : ToProblemResult(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteInstallmentPurchase(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteInstallmentPurchaseCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    private IActionResult ToProblemResult(Error error)
    {
        return error == InstallmentPurchaseErrors.NotFound
            ? NotFound(error)
            : BadRequest(error);
    }
}
