using Finance.Domain.Transactions;
using FluentValidation;

namespace Finance.Application.Vehicles.CreateVehicleRefuel;

internal sealed class CreateVehicleRefuelCommandValidator : AbstractValidator<CreateVehicleRefuelCommand>
{
    public CreateVehicleRefuelCommandValidator()
    {
        RuleFor(p => p.VehicleId).NotEmpty().WithMessage("Informe o veículo abastecido");
        RuleFor(p => p.Liters).GreaterThan(0).WithMessage("A quantidade de litros deve ser maior que zero");
        RuleFor(p => p.PricePerLiter).GreaterThan(0).WithMessage("O preço por litro deve ser maior que zero");
        RuleFor(p => p.CurrencyCode)
            .NotEmpty()
            .Must(code => Currency.All.Any(c => c.Code == code))
            .WithMessage("Informe uma moeda válida");
        RuleFor(p => p.Mileage).GreaterThan(0).WithMessage("Informe a quilometragem atual do veículo");
        RuleFor(p => p.CategoryId).NotEmpty().WithMessage("Informe a categoria do lançamento");
        RuleFor(p => p.ReleasedOnUtc).NotEmpty().WithMessage("Informe a data do abastecimento");
    }
}
