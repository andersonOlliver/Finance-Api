using Finance.Domain.Abstracts;

namespace Finance.Domain.Vehicles;

public static class VehicleErrors
{
    public static Error NotFound = new(
        "Vehicle.NotFound",
        "O veículo com o identificador informado não foi encontrado");

    public static Error MileageMustBeGreaterThanCurrent = new(
        "Vehicle.MileageMustBeGreaterThanCurrent",
        "A quilometragem informada deve ser maior que a quilometragem atual do veículo");
}
