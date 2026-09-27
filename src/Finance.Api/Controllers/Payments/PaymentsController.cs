using Finance.Application.Payments.CreatePayment;
using Finance.Application.Payments.DeletePayment;
using Finance.Application.Payments.GetPaymentById;
using Finance.Application.Payments.SearchPayments;
using Finance.Application.Payments.UpdatePayment;
using Finance.Domain.Abstracts;
using Finance.Domain.Payments;
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPaymentByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ToProblemResult(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePayment(CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePaymentCommand(request.Name, request.Type);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetPaymentById), new { id = result.Value }, result.Value)
            : ToProblemResult(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdatePayment(Guid id, UpdatePaymentRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdatePaymentCommand(id, request.Name, request.Type);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeletePayment(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePaymentCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    private IActionResult ToProblemResult(Error error)
    {
        return error == PaymentErrors.NotFound
            ? NotFound(error)
            : BadRequest(error);
    }
}
