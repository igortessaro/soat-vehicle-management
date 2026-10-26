using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Integration.Tests.Factories;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Integration.Tests.Tests.Vehicles;

public sealed class GetVehiclesTests : CustomWebApplicationFactory<Program>
{
    private const string Endpoint = "/api/v1/vehicles";

    [Test]
    public async Task Vehicles_GetOneAsync_WhenVehicleFound_ShouldReturn200()
    {
        // Arrange
        var vehicle = await SeedAsync(VehicleFactory.CreateVehicle());

        // Act
        var response = await Client.GetAsync($"{Endpoint}/{vehicle.Id}");
        var body = await ReadResponseAsync<VehicleDetail>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Id, Is.EqualTo(vehicle.Id));
            Assert.That(body.Data.LicensePlate, Is.EqualTo(vehicle.LicensePlate.Value));
            Assert.That(body.Data.Brand, Is.EqualTo(vehicle.Brand));
        });
    }

    [Test]
    public async Task Vehicles_GetOneAsync_WhenVehicleNotFound_ShouldReturn400()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"{Endpoint}/{id}");
        var error = await ReadErrorAsync(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.Select(x => x.Message), Has.Member($"Vehicle with id {id} not found"));
        });
    }

    [Test]
    public async Task Vehicles_GetOneAsync_WhenIdIsNotGuid_ShouldReturn404()
    {
        // Act
        var response = await Client.GetAsync($"{Endpoint}/not-a-guid");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}
