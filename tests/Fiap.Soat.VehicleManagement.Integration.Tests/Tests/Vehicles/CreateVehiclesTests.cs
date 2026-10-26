using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Fiap.Soat.VehicleManagement.Integration.Tests.Factories;
using System.Net;
using System.Net.Http.Json;

namespace Fiap.Soat.VehicleManagement.Integration.Tests.Tests.Vehicles;

public sealed class CreateVehiclesTests : CustomWebApplicationFactory<Program>
{
    private const string Endpoint = "/api/v1/vehicles";

    [Test]
    public async Task Vehicles_CreateAsync_WhenRequestIsValid_ShouldPersistAvailableVehicle()
    {
        // Arrange
        var request = new
        {
            Brand = Faker.Vehicle.Manufacturer(),
            Model = Faker.Vehicle.Model(),
            LicensePlate = VehicleFactory.CreateValidLicensePlate(),
            Year = DateTime.UtcNow.Year,
            Color = Faker.Commerce.Color(),
            Price = 150000m
        };

        // Act
        var response = await Client.PostAsJsonAsync(Endpoint, request);
        var body = await ReadResponseAsync<VehicleDetail>(response);
        var persisted = await FindAsync<Vehicle>(body.Data.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.IsSuccess, Is.True);
            Assert.That(body.Data.Status, Is.EqualTo(VehicleStatus.Available));
            Assert.That(persisted, Is.Not.Null);
            Assert.That(persisted!.LicensePlate.Value, Is.EqualTo(request.LicensePlate));
            Assert.That(persisted.Price, Is.EqualTo(request.Price));
        });
    }

    [TestCase("", "Corolla", "Black", 2024, 100000, "Brand is required")]
    [TestCase("Toyota", "", "Black", 2024, 100000, "Model is required")]
    [TestCase("Toyota", "Corolla", "", 2024, 100000, "Color is required")]
    [TestCase("Toyota", "Corolla", "Black", 1900, 100000, "Year must be a valid year")]
    [TestCase("Toyota", "Corolla", "Black", 2024, 0, "Price must be greater than zero")]
    public async Task Vehicles_CreateAsync_WhenRequestIsInvalid_ShouldReturn400(
        string brand, string model, string color, int year, decimal price, string expectedMessage)
    {
        // Arrange
        var request = new { Brand = brand, Model = model, LicensePlate = VehicleFactory.CreateValidLicensePlate(), Year = year, Color = color, Price = price };

        // Act
        var response = await Client.PostAsJsonAsync(Endpoint, request);
        var error = await ReadErrorAsync(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.Select(x => x.Message), Has.Member(expectedMessage));
        });
    }

    [Test]
    public async Task Vehicles_CreateAsync_WhenLicensePlateAlreadyExists_ShouldReturn400()
    {
        // Arrange
        var existing = await SeedAsync(VehicleFactory.CreateVehicle());
        var request = new
        {
            Brand = Faker.Vehicle.Manufacturer(),
            Model = Faker.Vehicle.Model(),
            LicensePlate = existing.LicensePlate.Value,
            Year = DateTime.UtcNow.Year,
            Color = Faker.Commerce.Color(),
            Price = 90000m
        };

        // Act
        var response = await Client.PostAsJsonAsync(Endpoint, request);
        var error = await ReadErrorAsync(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.Select(x => x.Message), Has.Member("Vehicle with this license plate already exists"));
        });
    }
}
