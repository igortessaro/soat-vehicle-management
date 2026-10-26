using Fiap.Soat.VehicleManagement.Domain.ValueObjects;

namespace Fiap.Soat.VehicleManagement.Domain.Entities;

public sealed class Vehicle : Entity
{
    private Vehicle() { }

    public Vehicle(string brand, string model, string licensePlate, int manufactureYear, string color)
    {
        Brand = brand;
        Model = model;
        LicensePlate = licensePlate;
        ManufactureYear = manufactureYear;
        Color = color;
    }

    public VehicleType Type { get; private set; } = VehicleType.Car;
    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public LicensePlate LicensePlate { get; private set; } = null!;
    public int ManufactureYear { get; private set; }
    public string Color { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public VehicleStatus Status { get; private set; } = VehicleStatus.Available;

    public Vehicle UpdatePrice(decimal price)
    {
        Price = price;
        return this;
    }

    public Vehicle Update(string brand, string model, string licensePlate, int year, string color, decimal price)
    {
        Brand = brand;
        Model = model;
        LicensePlate = licensePlate;
        ManufactureYear = year;
        Color = color;
        Price = price;
        return this;
    }

    public Vehicle Release()
    {
        Status = VehicleStatus.Available;
        return this;
    }

    public Vehicle Reserve()
    {
        Status = VehicleStatus.Reserved;
        return this;
    }

    public Vehicle Sold()
    {
        Status = VehicleStatus.Sold;
        return this;
    }

    public Vehicle Deliver()
    {
        Status = VehicleStatus.Delivered;
        return this;
    }
}
