using Finance.Application.CreditCards.CreateCreditCard;
using Finance.Application.CreditCards.DeleteCreditCard;
using Finance.Application.CreditCards.GetCreditCardById;
using Finance.Application.CreditCards.SearchCreditCards;
using Finance.Application.CreditCards.UpdateCreditCard;
using Finance.Domain.Abstracts;
using Finance.Domain.CreditCards;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.CreditCards;

[Authorize]
[ApiController]
[Route("api/credit-cards")]
public class CreditCardsController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchCreditCards(CancellationToken cancellationToken)
    {
        var query = new SearchCreditCardsQuery();
        var result = await sender.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCreditCardById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetCreditCardByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ToProblemResult(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCreditCard(CreateCreditCardRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateCreditCardCommand(request.Nickname, request.Brand, request.DueDay);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetCreditCardById), new { id = result.Value }, result.Value)
            : ToProblemResult(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCreditCard(Guid id, UpdateCreditCardRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCreditCardCommand(id, request.Nickname, request.Brand, request.DueDay);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCreditCard(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteCreditCardCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    private IActionResult ToProblemResult(Error error)
    {
        return error == CreditCardErrors.NotFound
            ? NotFound(error)
            : BadRequest(error);
    }
}
