namespace Fiap.Soat.VehicleManagement.Domain.Entities;

public sealed class Vehicle : Entity
{
    private const string TypeCar = "CAR";
    private const string AvailableStatus = "AVAILABLE";

    public Vehicle(string brand, string model, string licensePlate, int year, string color)
    {
        Brand = brand;
        Model = model;
        LicensePlate = licensePlate;
        Year = year;
        Color = color;
    }

    private Vehicle() { }

    public string Type { get; private set; } = TypeCar;
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public string LicensePlate { get; private set; }
    public int Year { get; private set; }
    public string Color { get; private set; }
    public decimal Price { get; private set; }
    public string Status { get; private set; } = AvailableStatus;

    public Vehicle UpdatePrice(decimal price)
    {
        Price = price;
        return this;
    }
}
