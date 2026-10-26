using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Infrastructure.Data;

namespace Fiap.Soat.VehicleManagement.Infrastructure.Tests;

public class UpdatedAtInterceptorTests
{
    private static readonly DateTime StaleTimestamp = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    [Test]
    public async Task SaveChangesAsync_WhenEntityIsAdded_KeepsOriginalUpdatedAt()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var entity = new Vehicle("Toyota", "Corolla", "ABC1234", 2024, "Black").UpdatePrice(100000);
        context.Vehicles.Add(entity);
        context.Entry(entity).Property(x => x.UpdatedAt).CurrentValue = StaleTimestamp;

        await context.SaveChangesAsync();

        Assert.That(entity.UpdatedAt, Is.EqualTo(StaleTimestamp));
    }

    [Test]
    public async Task SaveChangesAsync_WhenEntityIsModified_StampsUpdatedAt()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var entity = await AddWithStaleTimestampAsync(context);
        var beforeUpdate = DateTime.UtcNow;

        entity.UpdatePrice(120000);
        context.Vehicles.Update(entity);
        await context.SaveChangesAsync();

        Assert.That(entity.UpdatedAt, Is.GreaterThanOrEqualTo(beforeUpdate));
    }

    [Test]
    public async Task SaveChanges_WhenEntityIsModified_StampsUpdatedAt()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var entity = await AddWithStaleTimestampAsync(context);
        var beforeUpdate = DateTime.UtcNow;

        entity.UpdatePrice(120000);
        context.Vehicles.Update(entity);
        context.SaveChanges();

        Assert.That(entity.UpdatedAt, Is.GreaterThanOrEqualTo(beforeUpdate));
    }

    [Test]
    public async Task SaveChangesAsync_WhenEntityIsUnchanged_KeepsUpdatedAt()
    {
        await using var context = InfrastructureTestFixture.CreateContext();
        var entity = await AddWithStaleTimestampAsync(context);

        await context.SaveChangesAsync();

        Assert.That(entity.UpdatedAt, Is.EqualTo(StaleTimestamp));
    }

    private static async Task<Vehicle> AddWithStaleTimestampAsync(AppDbContext context)
    {
        var entity = new Vehicle("Toyota", "Corolla", "ABC1234", 2024, "Black").UpdatePrice(100000);
        context.Vehicles.Add(entity);
        context.Entry(entity).Property(x => x.UpdatedAt).CurrentValue = StaleTimestamp;
        await context.SaveChangesAsync();
        return entity;
    }
}
