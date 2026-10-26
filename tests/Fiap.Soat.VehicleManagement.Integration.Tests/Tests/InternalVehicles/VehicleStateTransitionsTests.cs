using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Fiap.Soat.VehicleManagement.Integration.Tests.Factories;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Integration.Tests.Tests.InternalVehicles;

public sealed class VehicleStateTransitionsTests : CustomWebApplicationFactory<Program>
{
    private const string Endpoint = "/api/v1/internal/vehicles";

    [TestCase(VehicleStatus.Available, "reserve", VehicleStatus.Reserved)]
    [TestCase(VehicleStatus.Reserved, "release", VehicleStatus.Available)]
    [TestCase(VehicleStatus.Reserved, "sell", VehicleStatus.Sold)]
    [TestCase(VehicleStatus.Sold, "deliver", VehicleStatus.Delivered)]
    public async Task InternalVehicles_Transition_WhenAllowed_ShouldPersistNewStatus(
        VehicleStatus initialStatus, string operation, VehicleStatus expectedStatus)
    {
        // Arrange
        var vehicle = await SeedAsync(VehicleFactory.CreateVehicle(initialStatus));

        // Act
        var response = await Client.PostAsync($"{Endpoint}/{vehicle.Id}/{operation}", null);
        var body = await ReadResponseAsync<VehicleDetail>(response);
        var persisted = await FindAsync<Vehicle>(vehicle.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Status, Is.EqualTo(expectedStatus));
            Assert.That(persisted!.Status, Is.EqualTo(expectedStatus));
        });
    }

    [TestCase(VehicleStatus.Sold, "reserve", "Is not possible to reserve a vehicle that is not available or reserved")]
    [TestCase(VehicleStatus.Sold, "release", "Is not possible to release a vehicle that is not sold or delivered")]
    [TestCase(VehicleStatus.Available, "sell", "Is not possible to sell a vehicle that is not reserved or sold")]
    [TestCase(VehicleStatus.Reserved, "deliver", "Is not possible to deliver a vehicle that is not sold or delivered")]
    public async Task InternalVehicles_Transition_WhenNotAllowed_ShouldReturn400AndKeepStatus(
        VehicleStatus initialStatus, string operation, string expectedMessage)
    {
        // Arrange
        var vehicle = await SeedAsync(VehicleFactory.CreateVehicle(initialStatus));

        // Act
        var response = await Client.PostAsync($"{Endpoint}/{vehicle.Id}/{operation}", null);
        var error = await ReadErrorAsync(response);
        var persisted = await FindAsync<Vehicle>(vehicle.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.Select(x => x.Message), Has.Member(expectedMessage));
            Assert.That(persisted!.Status, Is.EqualTo(initialStatus));
        });
    }

    [Test]
    public async Task InternalVehicles_SaleFlow_ShouldMoveVehicleFromAvailableToDelivered()
    {
        // Arrange
        var vehicle = await SeedAsync(VehicleFactory.CreateVehicle());

        // Act
        var reserve = await Client.PostAsync($"{Endpoint}/{vehicle.Id}/reserve", null);
        var sell = await Client.PostAsync($"{Endpoint}/{vehicle.Id}/sell", null);
        var deliver = await Client.PostAsync($"{Endpoint}/{vehicle.Id}/deliver", null);
        var persisted = await FindAsync<Vehicle>(vehicle.Id);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(new[] { reserve.StatusCode, sell.StatusCode, deliver.StatusCode }, Is.All.EqualTo(HttpStatusCode.OK));
            Assert.That(persisted!.Status, Is.EqualTo(VehicleStatus.Delivered));
        });
    }

    [TestCase("reserve")]
    [TestCase("release")]
    [TestCase("sell")]
    [TestCase("deliver")]
    public async Task InternalVehicles_Transition_WhenVehicleNotFound_ShouldReturn400(string operation)
    {
        // Act
        var response = await Client.PostAsync($"{Endpoint}/{Guid.NewGuid()}/{operation}", null);
        var error = await ReadErrorAsync(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.Select(x => x.Message), Has.Member("Vehicle not found"));
        });
    }
}
