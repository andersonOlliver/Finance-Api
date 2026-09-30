using FluentValidation;

namespace Finance.Application.Vehicles.CreateVehicle;

internal sealed class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(p => p.Nickname).NotEmpty().MaximumLength(100).WithMessage("Informe um nome/apelido válido para o veículo");
        RuleFor(p => p.Type).IsInEnum().WithMessage("Informe um tipo de veículo válido");
        RuleFor(p => p.Brand).NotEmpty().MaximumLength(100).WithMessage("Informe a marca do veículo");
        RuleFor(p => p.Model).NotEmpty().MaximumLength(100).WithMessage("Informe o modelo do veículo");
        RuleFor(p => p.Color).NotEmpty().MaximumLength(50).WithMessage("Informe a cor do veículo");
        RuleFor(p => p.Year).InclusiveBetween(1900, DateTime.UtcNow.Year + 1).WithMessage("Informe um ano válido");
        RuleFor(p => p.LicensePlate).NotEmpty().MaximumLength(10).WithMessage("Informe a placa do veículo");
        RuleFor(p => p.InitialMileage).GreaterThanOrEqualTo(0).WithMessage("A quilometragem inicial não pode ser negativa");
    }
}
