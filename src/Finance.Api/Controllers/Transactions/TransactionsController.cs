using Finance.Application.Transactions.CreateTransaction;
using Finance.Application.Transactions.DeleteTransaction;
using Finance.Application.Transactions.GetTransactionById;
using Finance.Application.Transactions.SearchTransactions;
using Finance.Application.Transactions.UpdateTransaction;
using Finance.Domain.Abstracts;
using Finance.Domain.Transactions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Transactions;

[Authorize]
[ApiController]
[Route("api/transactions")]
public class TransactionsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchTransactions(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? paymentId,
        CancellationToken cancellationToken)
    {
        var query = new SearchTransactionsQuery(from, to, categoryId, paymentId);
        var result = await sender.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTransactionById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetTransactionByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ToProblemResult(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTransactionCommand(
            request.Title,
            request.Amount,
            request.CurrencyCode,
            request.Description,
            request.CategoryId,
            request.PaymentId,
            request.ReleasedOnUtc);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetTransactionById), new { id = result.Value }, result.Value)
            : ToProblemResult(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTransaction(Guid id, UpdateTransactionRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTransactionCommand(
            id,
            request.Title,
            request.Amount,
            request.CurrencyCode,
            request.Description,
            request.CategoryId,
            request.PaymentId,
            request.ReleasedOnUtc);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteTransactionCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    private IActionResult ToProblemResult(Error error)
    {
        return error == TransactionErrors.NotFound
            ? NotFound(error)
            : BadRequest(error);
    }
}
