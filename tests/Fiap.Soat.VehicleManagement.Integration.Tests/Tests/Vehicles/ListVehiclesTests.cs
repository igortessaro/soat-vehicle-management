using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Fiap.Soat.VehicleManagement.Integration.Tests.Factories;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Integration.Tests.Tests.Vehicles;

public sealed class ListVehiclesTests : CustomWebApplicationFactory<Program>
{
    private const string Endpoint = "/api/v1/vehicles";

    private Vehicle _available = null!;
    private Vehicle _cheaperAvailable = null!;
    private Vehicle _reserved = null!;
    private Vehicle _sold = null!;
    private Vehicle _delivered = null!;

    [OneTimeSetUp]
    public async Task SeedVehicles()
    {
        _available = await SeedAsync(VehicleFactory.CreateVehicle(brand: "Toyota", color: "Black", year: 2024, price: 120000));
        _cheaperAvailable = await SeedAsync(VehicleFactory.CreateVehicle(brand: "Toyota", color: "White", year: 2022, price: 80000));
        _reserved = await SeedAsync(VehicleFactory.CreateVehicle(VehicleStatus.Reserved, brand: "Honda", price: 90000));
        _sold = await SeedAsync(VehicleFactory.CreateVehicle(VehicleStatus.Sold, brand: "Honda", price: 100000));
        _delivered = await SeedAsync(VehicleFactory.CreateVehicle(VehicleStatus.Delivered, brand: "Fiat", price: 70000));
    }

    [Test]
    public async Task Vehicles_ListAsync_WithoutFilters_ShouldReturnAllVehiclesOrderedByPrice()
    {
        // Act
        var response = await Client.GetAsync(Endpoint);
        var body = await ReadResponseAsync<List<VehicleDetail>>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Select(x => x.Price), Is.Ordered.Ascending);
            Assert.That(body.Data.Select(x => x.Id), Is.EquivalentTo(new[] { _available.Id, _cheaperAvailable.Id, _reserved.Id, _sold.Id, _delivered.Id }));
        });
    }

    [Test]
    public async Task Vehicles_ListAsync_WithFilters_ShouldReturnOnlyMatchingVehicles()
    {
        // Act
        var response = await Client.GetAsync($"{Endpoint}?Brand=Toyota&Color=Black&Year=2024&Status=Available");
        var body = await ReadResponseAsync<List<VehicleDetail>>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Single().Id, Is.EqualTo(_available.Id));
        });
    }

    [Test]
    public async Task Vehicles_ListAvailableAsync_ShouldReturnOnlyAvailableVehiclesOrderedByPrice()
    {
        // Act
        var response = await Client.GetAsync($"{Endpoint}/available");
        var body = await ReadResponseAsync<List<VehicleDetail>>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Select(x => x.Id), Is.EqualTo(new[] { _cheaperAvailable.Id, _available.Id }));
        });
    }

    [Test]
    public async Task Vehicles_ListSoldAsync_ShouldReturnSoldAndDeliveredVehicles()
    {
        // Act
        var response = await Client.GetAsync($"{Endpoint}/sold?Status=Available");
        var body = await ReadResponseAsync<List<VehicleDetail>>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Select(x => x.Id), Is.EqualTo(new[] { _delivered.Id, _sold.Id }));
            Assert.That(body.Data.Select(x => x.Status), Is.All.AnyOf(VehicleStatus.Sold, VehicleStatus.Delivered));
        });
    }
}
