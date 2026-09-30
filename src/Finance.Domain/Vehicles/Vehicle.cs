using Finance.Domain.Abstracts;
using Finance.Domain.Shared;

namespace Finance.Domain.Vehicles;

public sealed class Vehicle : Entity
{
    private Vehicle(
        Guid id,
        Name nickname,
        VehicleType type,
        string brand,
        string model,
        string color,
        int year,
        string licensePlate,
        decimal mileage,
        Guid userId,
        DateTime createdOnUtc)
        : base(id)
    {
        Nickname = nickname;
        Type = type;
        Brand = brand;
        Model = model;
        Color = color;
        Year = year;
        LicensePlate = licensePlate;
        Mileage = mileage;
        UserId = userId;
        CreatedOnUtc = createdOnUtc;
    }

    private Vehicle() { }

    public Name Nickname { get; private set; }
    public VehicleType Type { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string Color { get; private set; }
    public int Year { get; private set; }
    public string LicensePlate { get; private set; }
    public decimal Mileage { get; private set; }
    public Guid UserId { get; init; }
    public DateTime CreatedOnUtc { get; init; }
    public DateTime? UpdatedOnUtc { get; private set; }

    public static Vehicle Create(
        Guid id,
        Name nickname,
        VehicleType type,
        string brand,
        string model,
        string color,
        int year,
        string licensePlate,
        decimal initialMileage,
        Guid userId,
        DateTime createdOnUtc)
    {
        return new Vehicle(id, nickname, type, brand, model, color, year, licensePlate, initialMileage, userId, createdOnUtc);
    }

    public void Update(
        Name nickname,
        VehicleType type,
        string brand,
        string model,
        string color,
        int year,
        string licensePlate,
        DateTime updatedOnUtc)
    {
        Nickname = nickname;
        Type = type;
        Brand = brand;
        Model = model;
        Color = color;
        Year = year;
        LicensePlate = licensePlate;
        UpdatedOnUtc = updatedOnUtc;
    }

    public void UpdateMileage(decimal mileage, DateTime updatedOnUtc)
    {
        Mileage = mileage;
        UpdatedOnUtc = updatedOnUtc;
    }
}
