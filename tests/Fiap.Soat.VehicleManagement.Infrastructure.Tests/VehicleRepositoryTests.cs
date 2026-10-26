using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Fiap.Soat.VehicleManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Soat.VehicleManagement.Infrastructure.Tests;

public class VehicleRepositoryTests
{
    private static readonly decimal[] ExpectedPricesInAscendingOrder = [90000m, 120000m];

    [Test]
    public async Task AddAndGetByIdAsync_PersistsVehicle()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        var vehicle = CreateVehicle();

        var added = await repository.AddAsync(vehicle, CancellationToken.None);
        var result = await repository.GetByIdAsync(vehicle.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(added, Is.SameAs(vehicle));
            Assert.That(result!.Id, Is.EqualTo(vehicle.Id));
        });
    }

    [Test]
    public async Task GetDetailedByIdAsync_ReturnsDetachedVehicle()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        var vehicle = await AddVehicle(context, "Toyota", "Corolla", 100000);

        var result = await repository.GetDetailedByIdAsync(vehicle.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(context.Entry(result!).State, Is.EqualTo(EntityState.Detached));
        });
    }

    [Test]
    public async Task GetAllAsync_AppliesFiltersAndOrdersByPrice()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        await AddVehicle(context, "Toyota", "Corolla", 120000, VehicleStatus.Available, "Black");
        await AddVehicle(context, "Honda", "Civic", 80000, VehicleStatus.Reserved, "White");
        await AddVehicle(context, "Toyota", "Yaris", 90000, VehicleStatus.Available, "Red");

        var result = await repository.GetAllAsync(
            null, null, "Toyota", null, null, [VehicleStatus.Available], CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Select(x => x.Price), Is.EqualTo(ExpectedPricesInAscendingOrder));
        });
    }

    [Test]
    public async Task GetAllAsync_AppliesAllFilters()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        var expected = await AddVehicle(context, "Toyota", "Corolla", 100000, plate: "ABC1234");
        await AddVehicle(context, "Toyota", "Corolla", 100000, VehicleStatus.Sold, "Black", 2024, "XYZ9876");

        var result = await repository.GetAllAsync(
            "ABC1234", "Corolla", "Toyota", "Black", 2024, [VehicleStatus.Available], CancellationToken.None);

        Assert.That(result.Single().Id, Is.EqualTo(expected.Id));
    }

    [Test]
    public async Task GetByLicensePlateAsync_ReturnsMatchingVehicleOrNull()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        var vehicle = await AddVehicle(context, "Toyota", "Corolla", 100000, plate: "ABC1234");

        var found = await repository.GetByLicensePlateAsync("ABC1234", CancellationToken.None);
        var missing = await repository.GetByLicensePlateAsync("NOTFOUND", CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(found!.Id, Is.EqualTo(vehicle.Id));
            Assert.That(missing, Is.Null);
        });
    }

    [Test]
    public async Task UpdateAsync_UpdatesVehicle()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        var vehicle = await AddVehicle(context, "Toyota", "Corolla", 100000);
        vehicle.Update("Honda", "Civic", "XYZ9876", 2023, "White", 120000);

        await repository.UpdateAsync(vehicle, CancellationToken.None);
        context.ChangeTracker.Clear();
        var result = await repository.GetByIdAsync(vehicle.Id, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result!.Brand, Is.EqualTo("Honda"));
            Assert.That(result.Model, Is.EqualTo("Civic"));
            Assert.That(result.Price, Is.EqualTo(120000));
        });
    }

    [Test]
    public async Task DeleteAndDeleteRangeAsync_RemoveVehicles()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        var first = await AddVehicle(context, "Toyota", "Corolla", 100000);
        var second = await AddVehicle(context, "Honda", "Civic", 90000, plate: "XYZ9876");

        await repository.DeleteAsync(first, CancellationToken.None);
        await repository.DeleteRangeAsync([second], CancellationToken.None);

        Assert.That(await repository.GetAllAsync(null, null, null, null, null, [], CancellationToken.None), Is.Empty);
    }

    [Test]
    public async Task FindAndAnyAsync_ReturnExpectedResults()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        var vehicle = await AddVehicle(context, "Toyota", "Corolla", 100000);

        var found = await repository.FindAsync(x => x.Brand == "Toyota", CancellationToken.None);
        var single = await repository.FindSingleAsync(x => x.Model == "Corolla", CancellationToken.None);

        var hasToyota = await repository.AnyAsync(x => x.Brand == "Toyota", CancellationToken.None);
        var hasHonda = await repository.AnyAsync(x => x.Brand == "Honda", CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(found.Single().Id, Is.EqualTo(vehicle.Id));
            Assert.That(single!.Id, Is.EqualTo(vehicle.Id));
            Assert.That(hasToyota, Is.True);
            Assert.That(hasHonda, Is.False);
        });
    }

    [Test]
    public async Task GetAllPaginatedAsync_ReturnsMetadataAndPage()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        await AddVehicle(context, "Toyota", "One", 100000);
        await AddVehicle(context, "Toyota", "Two", 80000, plate: "XYZ9876");
        await AddVehicle(context, "Toyota", "Three", 90000, plate: "DEF5678");

        var result = await repository.GetAllAsync(
            new PaginatedRequest { PageNumber = 2, PageSize = 2 },
            CancellationToken.None,
            query => query.OrderBy(x => x.Price));

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalCount, Is.EqualTo(3));
            Assert.That(result.TotalPages, Is.EqualTo(2));
            Assert.That(result.Items.Single().Model, Is.EqualTo("One"));
        });
    }

    [Test]
    public async Task GetAllPaginatedAsync_WhenPageIsZero_ReturnsNoItems()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        await AddVehicle(context, "Toyota", "Corolla", 100000);

        var result = await repository.GetAllAsync(
            new PaginatedRequest { PageNumber = 0, PageSize = 10 }, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result.TotalCount, Is.EqualTo(1));
            Assert.That(result.Items, Is.Empty);
            Assert.That(result.TotalPages, Is.EqualTo(1));
        });
    }

    [Test]
    public async Task GetAllPaginatedAsync_SupportsPredicateAndIncludesOverloads()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);
        await AddVehicle(context, "Toyota", "Corolla", 100000);
        await AddVehicle(context, "Honda", "Civic", 90000, plate: "XYZ9876");
        var request = new PaginatedRequest { PageNumber = 1, PageSize = 10 };

        var byPredicate = await repository.GetAllAsync(
            x => x.Brand == "Toyota", request, CancellationToken.None);
        var byIncludes = await repository.GetAllAsync(
            Array.Empty<string>(), request, CancellationToken.None);
        var byIncludesAndPredicate = await repository.GetAllAsync(
            Array.Empty<string>(), x => x.Brand == "Honda", request, CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(byPredicate.Items.Single().Brand, Is.EqualTo("Toyota"));
            Assert.That(byIncludes.TotalCount, Is.EqualTo(2));
            Assert.That(byIncludesAndPredicate.Items.Single().Brand, Is.EqualTo("Honda"));
        });
    }

    [Test]
    public void GetDbConnection_ThrowsForNonRelationalProvider()
    {
        using var context = InfrastructureTestFixture.CreateContext();
        var repository = new VehicleRepository(context);

        Assert.That(
            () => repository.GetDbConnection(),
            Throws.InvalidOperationException);
    }

    private static Vehicle CreateVehicle(
        string brand = "Toyota",
        string model = "Corolla",
        decimal price = 100000,
        VehicleStatus status = VehicleStatus.Available,
        string color = "Black",
        int year = 2024,
        string plate = "ABC1234")
    {
        var vehicle = new Vehicle(brand, model, plate, year, color).UpdatePrice(price);
        return status switch
        {
            VehicleStatus.Reserved => vehicle.Reserve(),
            VehicleStatus.Sold => vehicle.Reserve().Sold(),
            VehicleStatus.Delivered => vehicle.Reserve().Sold().Deliver(),
            _ => vehicle
        };
    }

    private static async Task<Vehicle> AddVehicle(
        Fiap.Soat.VehicleManagement.Infrastructure.Data.AppDbContext context,
        string brand,
        string model,
        decimal price,
        VehicleStatus status = VehicleStatus.Available,
        string color = "Black",
        int year = 2024,
        string plate = "ABC1234")
    {
        var vehicle = CreateVehicle(brand, model, price, status, color, year, plate);
        context.Vehicles.Add(vehicle);
        await context.SaveChangesAsync();
        return vehicle;
    }
}
