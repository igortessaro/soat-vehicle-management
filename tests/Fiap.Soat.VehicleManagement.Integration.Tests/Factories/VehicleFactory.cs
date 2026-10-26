using Bogus;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;

namespace Fiap.Soat.VehicleManagement.Integration.Tests.Factories;

public static class VehicleFactory
{
    private static readonly Faker Faker = new("pt_BR");

    public static string CreateValidLicensePlate() => Faker.Random.Replace("???#?##").ToUpperInvariant();

    public static Vehicle CreateVehicle(
        VehicleStatus status = VehicleStatus.Available,
        string? brand = null,
        string? model = null,
        string? color = null,
        int? year = null,
        decimal? price = null)
    {
        var vehicle = new Vehicle(
                brand ?? Faker.Vehicle.Manufacturer(),
                model ?? Faker.Vehicle.Model(),
                CreateValidLicensePlate(),
                year ?? Faker.Random.Int(2015, DateTime.UtcNow.Year),
                color ?? Faker.Commerce.Color())
            .UpdatePrice(price ?? Faker.Random.Decimal(30000, 300000));

        return status switch
        {
            VehicleStatus.Reserved => vehicle.Reserve(),
            VehicleStatus.Sold => vehicle.Reserve().Sold(),
            VehicleStatus.Delivered => vehicle.Reserve().Sold().Deliver(),
            _ => vehicle
        };
    }
}
