using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Fiap.Soat.VehicleManagement.Infrastructure.Data;
using Fiap.Soat.VehicleManagement.Integration.Tests.Factories;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Integration.Tests.Tests.Vehicles;

public sealed class ListVehiclesUpdatedSinceTests : CustomWebApplicationFactory<Program>
{
    private const string Endpoint = "/api/v1/vehicles";
    private static readonly DateTime UpdatedSince = new(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc);

    private Vehicle _old = null!;
    private Vehicle _shortlyBeforeCutoff = null!;
    private Vehicle _atCutoff = null!;
    private Vehicle _recentAvailable = null!;
    private Vehicle _recentSold = null!;

    [OneTimeSetUp]
    public async Task SeedVehicles()
    {
        _old = await SeedWithUpdatedAtAsync(VehicleFactory.CreateVehicle(price: 100000), UpdatedSince.AddDays(-1));
        // 22:00 UTC is before the cutoff, but after 21:00 if the -03:00 offset were ignored
        _shortlyBeforeCutoff = await SeedWithUpdatedAtAsync(VehicleFactory.CreateVehicle(price: 110000), UpdatedSince.AddHours(-2));
        _atCutoff = await SeedWithUpdatedAtAsync(VehicleFactory.CreateVehicle(price: 120000), UpdatedSince);
        _recentAvailable = await SeedWithUpdatedAtAsync(VehicleFactory.CreateVehicle(price: 90000), UpdatedSince.AddHours(1));
        _recentSold = await SeedWithUpdatedAtAsync(VehicleFactory.CreateVehicle(VehicleStatus.Sold, price: 80000), UpdatedSince.AddHours(2));
    }

    [Test]
    public async Task Vehicles_ListAsync_WithUpdatedSince_ShouldReturnVehiclesUpdatedFromThatMomentOrderedByPrice()
    {
        // Act
        var response = await Client.GetAsync($"{Endpoint}?updatedSince=2026-01-02T00:00:00Z");
        var body = await ReadResponseAsync<List<VehicleDetail>>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Select(x => x.Id), Is.EqualTo(new[] { _recentSold.Id, _recentAvailable.Id, _atCutoff.Id }));
            Assert.That(body.Data.Select(x => x.Id), Does.Not.Contain(_old.Id).And.Not.Contain(_shortlyBeforeCutoff.Id));
        });
    }

    [Test]
    public async Task Vehicles_ListAsync_WithUpdatedSinceInAnotherOffset_ShouldCompareInUtc()
    {
        // Act: 21:00 at -03:00 is the same instant as 00:00 UTC of the next day
        var response = await Client.GetAsync($"{Endpoint}?updatedSince=2026-01-01T21:00:00-03:00");
        var body = await ReadResponseAsync<List<VehicleDetail>>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Select(x => x.Id), Is.EquivalentTo(new[] { _recentSold.Id, _recentAvailable.Id, _atCutoff.Id }));
        });
    }

    [Test]
    public async Task Vehicles_ListAvailableAsync_WithUpdatedSince_ShouldCombineBothFilters()
    {
        // Act
        var response = await Client.GetAsync($"{Endpoint}/available?updatedSince=2026-01-02T00:30:00Z");
        var body = await ReadResponseAsync<List<VehicleDetail>>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data.Single().Id, Is.EqualTo(_recentAvailable.Id));
        });
    }

    [Test]
    public async Task Vehicles_ListAsync_WithoutUpdatedSince_ShouldReturnAllVehicles()
    {
        // Act
        var response = await Client.GetAsync(Endpoint);
        var body = await ReadResponseAsync<List<VehicleDetail>>(response);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(body.Data, Has.Count.EqualTo(5));
        });
    }

    [Test]
    public async Task Vehicles_ListAsync_WithInvalidUpdatedSince_ShouldReturn400()
    {
        // Act
        var response = await Client.GetAsync($"{Endpoint}?updatedSince=not-a-date");

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }

    private async Task<Vehicle> SeedWithUpdatedAtAsync(Vehicle vehicle, DateTime updatedAt)
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Vehicles.AddAsync(vehicle);
        dbContext.Entry(vehicle).Property(x => x.UpdatedAt).CurrentValue = updatedAt;
        await dbContext.SaveChangesAsync();
        return vehicle;
    }
}
