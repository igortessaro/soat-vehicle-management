using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;

namespace Fiap.Soat.VehicleManagement.Domain.Tests;

public class VehicleTests
{
    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        var vehicle = new Vehicle("Toyota", "Corolla", "abc1d23", 2024, "Black");

        Assert.Multiple(() =>
        {
            Assert.That(vehicle.Id, Is.EqualTo(Guid.Empty));
            Assert.That(vehicle.Type, Is.EqualTo(VehicleType.Car));
            Assert.That(vehicle.Brand, Is.EqualTo("Toyota"));
            Assert.That(vehicle.Model, Is.EqualTo("Corolla"));
            Assert.That(vehicle.LicensePlate.Value, Is.EqualTo("ABC1D23"));
            Assert.That(vehicle.ManufactureYear, Is.EqualTo(2024));
            Assert.That(vehicle.Color, Is.EqualTo("Black"));
            Assert.That(vehicle.Price, Is.Zero);
            Assert.That(vehicle.Status, Is.EqualTo(VehicleStatus.Available));
        });
    }

    [Test]
    public void UpdatePrice_ShouldChangePriceAndReturnSameVehicle()
    {
        var vehicle = CreateVehicle();
        var beforeUpdate = DateTime.UtcNow;

        var result = vehicle.UpdatePrice(125000m);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(vehicle));
            Assert.That(vehicle.Price, Is.EqualTo(125000m));
            Assert.That(vehicle.UpdatedAt, Is.GreaterThanOrEqualTo(beforeUpdate));
        });
    }

    [Test]
    public void Update_ShouldReplaceAllMutablePropertiesAndReturnSameVehicle()
    {
        var vehicle = CreateVehicle();
        var beforeUpdate = DateTime.UtcNow;

        var result = vehicle.Update("Honda", "Civic", "xyz9a99", 2025, "White", 130000m);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.SameAs(vehicle));
            Assert.That(vehicle.Brand, Is.EqualTo("Honda"));
            Assert.That(vehicle.Model, Is.EqualTo("Civic"));
            Assert.That(vehicle.LicensePlate.Value, Is.EqualTo("XYZ9A99"));
            Assert.That(vehicle.ManufactureYear, Is.EqualTo(2025));
            Assert.That(vehicle.Color, Is.EqualTo("White"));
            Assert.That(vehicle.Price, Is.EqualTo(130000m));
            Assert.That(vehicle.UpdatedAt, Is.GreaterThanOrEqualTo(beforeUpdate));
        });
    }

    [Test]
    public void StateTransitions_ShouldMarkVehicleAsUpdated()
    {
        var vehicle = CreateVehicle();
        Func<Vehicle>[] transitions = [vehicle.Reserve, vehicle.Sold, vehicle.Deliver, vehicle.Release];

        Assert.Multiple(() =>
        {
            foreach (var transition in transitions)
            {
                var beforeTransition = DateTime.UtcNow;
                transition();
                Assert.That(vehicle.UpdatedAt, Is.GreaterThanOrEqualTo(beforeTransition));
            }
        });
    }

    [Test]
    public void StateTransitions_ShouldSetExpectedStatuses()
    {
        var vehicle = CreateVehicle();

        Assert.Multiple(() =>
        {
            Assert.That(vehicle.Reserve(), Is.SameAs(vehicle));
            Assert.That(vehicle.Status, Is.EqualTo(VehicleStatus.Reserved));
            Assert.That(vehicle.Sold(), Is.SameAs(vehicle));
            Assert.That(vehicle.Status, Is.EqualTo(VehicleStatus.Sold));
            Assert.That(vehicle.Deliver(), Is.SameAs(vehicle));
            Assert.That(vehicle.Status, Is.EqualTo(VehicleStatus.Delivered));
            Assert.That(vehicle.Release(), Is.SameAs(vehicle));
            Assert.That(vehicle.Status, Is.EqualTo(VehicleStatus.Available));
        });
    }

    private static Vehicle CreateVehicle() =>
        new("Toyota", "Corolla", "ABC1D23", 2024, "Black");
}
