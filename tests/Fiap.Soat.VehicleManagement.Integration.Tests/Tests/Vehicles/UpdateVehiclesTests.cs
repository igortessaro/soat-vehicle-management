using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Integration.Tests.Factories;
using System.Net;
using System.Net.Http.Json;

namespace Fiap.Soat.VehicleManagement.Integration.Tests.Tests.Vehicles;

public sealed class UpdateVehiclesTests : CustomWebApplicationFactory<Program>
{
    private const string Endpoint = "/api/v1/vehicles";

    [Test]
    public async Task Vehicles_UpdateAsync_WhenRequestIsValid_ShouldPersistChanges()
    {
        // Arrange
        var vehicle = await SeedAsync(VehicleFactory.CreateVehicle());
        var request = new
        {
            Brand = "Honda",
            Model = "Civic",
            LicensePlate = VehicleFactory.CreateValidLicensePlate(),
            Year = 2023,
            Color = "White",
            Price = 135000m
        };

        // Act
        var response = await Client.PutAsJsonAsync($"{Endpoint}/{vehicle.Id}", request);
        var body = await ReadResponseAsync<VehicleDetail>(response);
        var persisted = await FindAsync<Vehicle>(vehicle.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Brand, Is.EqualTo("Honda"));
            Assert.That(persisted!.Model, Is.EqualTo("Civic"));
            Assert.That(persisted.LicensePlate.Value, Is.EqualTo(request.LicensePlate));
            Assert.That(persisted.ManufactureYear, Is.EqualTo(2023));
            Assert.That(persisted.Price, Is.EqualTo(135000m));
        });
    }

    [Test]
    public async Task Vehicles_UpdateAsync_WhenVehicleNotFound_ShouldReturn400()
    {
        // Arrange
        var request = new { Brand = "Honda", Model = "Civic", LicensePlate = VehicleFactory.CreateValidLicensePlate(), Year = 2023, Color = "White", Price = 135000m };

        // Act
        var response = await Client.PutAsJsonAsync($"{Endpoint}/{Guid.NewGuid()}", request);
        var error = await ReadErrorAsync(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.Select(x => x.Message), Has.Member("Vehicle not found"));
        });
    }

    [Test]
    public async Task Vehicles_UpdateAsync_WhenPriceIsInvalid_ShouldReturn400AndKeepVehicle()
    {
        // Arrange
        var vehicle = await SeedAsync(VehicleFactory.CreateVehicle(price: 100000));
        var request = new { Brand = "Honda", Model = "Civic", LicensePlate = VehicleFactory.CreateValidLicensePlate(), Year = 2023, Color = "White", Price = 0m };

        // Act
        var response = await Client.PutAsJsonAsync($"{Endpoint}/{vehicle.Id}", request);
        var error = await ReadErrorAsync(response);
        var persisted = await FindAsync<Vehicle>(vehicle.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.Select(x => x.Message), Has.Member("Price must be greater than zero"));
            Assert.That(persisted!.Price, Is.EqualTo(100000m));
        });
    }
}
