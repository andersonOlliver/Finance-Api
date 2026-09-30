using Finance.Application.Vehicles.CreateVehicle;
using Finance.Application.Vehicles.CreateVehicleRefuel;
using Finance.Application.Vehicles.DeleteVehicle;
using Finance.Application.Vehicles.GetVehicleById;
using Finance.Application.Vehicles.SearchVehicleRefuels;
using Finance.Application.Vehicles.SearchVehicles;
using Finance.Application.Vehicles.UpdateVehicle;
using Finance.Domain.Abstracts;
using Finance.Domain.Vehicles;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Finance.Api.Controllers.Vehicles;

[Authorize]
[ApiController]
[Route("api/vehicles")]
public class VehiclesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> SearchVehicles(CancellationToken cancellationToken)
    {
        var query = new SearchVehiclesQuery();
        var result = await sender.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetVehicleById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetVehicleByIdQuery(id);
        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ToProblemResult(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateVehicle(CreateVehicleRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateVehicleCommand(
            request.Nickname,
            request.Type,
            request.Brand,
            request.Model,
            request.Color,
            request.Year,
            request.LicensePlate,
            request.InitialMileage);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetVehicleById), new { id = result.Value }, result.Value)
            : ToProblemResult(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateVehicle(Guid id, UpdateVehicleRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateVehicleCommand(
            id,
            request.Nickname,
            request.Type,
            request.Brand,
            request.Model,
            request.Color,
            request.Year,
            request.LicensePlate);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteVehicle(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteVehicleCommand(id);
        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? NoContent() : ToProblemResult(result.Error);
    }

    [HttpGet("{id:guid}/refuels")]
    public async Task<IActionResult> SearchRefuels(Guid id, CancellationToken cancellationToken)
    {
        var query = new SearchVehicleRefuelsQuery(id);
        var result = await sender.Send(query, cancellationToken);
        return Ok(result.Value);
    }

    [HttpPost("{id:guid}/refuels")]
    public async Task<IActionResult> CreateRefuel(Guid id, CreateVehicleRefuelRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateVehicleRefuelCommand(
            id,
            request.Liters,
            request.PricePerLiter,
            request.CurrencyCode,
            request.Mileage,
            request.CategoryId,
            request.PaymentId,
            request.Description,
            request.ReleasedOnUtc);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : ToProblemResult(result.Error);
    }

    private IActionResult ToProblemResult(Error error)
    {
        return error == VehicleErrors.NotFound
            ? NotFound(error)
            : BadRequest(error);
    }
}
