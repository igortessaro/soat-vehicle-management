using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Moq;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Application.Tests;

public class UpdateVehicleHandlerTests
{
    private readonly Mock<IVehicleRepository> _repository = new();

    [SetUp]
    public void SetUp() => _repository.Reset();

    [Test]
    public async Task Update_WhenRequestIsValid_UpdatesAndReturnsVehicle()
    {
        var id = Guid.NewGuid();
        var vehicle = ApplicationTestData.CreateVehicle();
        _repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);
        _repository.Setup(x => x.UpdateAsync(vehicle, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);

        var response = await new UpdateVehicleHandler(_repository.Object).Handle(
            new UpdateVehicleCommand(id, "Honda", "Civic", "XYZ-9876", 2023, "White", 120000),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data, Is.SameAs(vehicle));
            Assert.That(vehicle.Brand, Is.EqualTo("Honda"));
            Assert.That(vehicle.Price, Is.EqualTo(120000));
        });
        _repository.Verify(x => x.UpdateAsync(vehicle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Update_WhenVehicleDoesNotExist_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Vehicle?) null);

        var response = await new UpdateVehicleHandler(_repository.Object).Handle(
            new UpdateVehicleCommand(id, "Honda", "Civic", "XYZ9876", 2023, "White", 120000),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Reasons[0].Message, Is.EqualTo("Vehicle not found"));
        });
        _repository.Verify(x => x.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestCase("", "Civic", "XYZ9876", 2023, "White", 120000, "Brand is required")]
    [TestCase("Honda", "", "XYZ9876", 2023, "White", 120000, "Model is required")]
    [TestCase("Honda", "Civic", "", 2023, "White", 120000, "License plate is required")]
    [TestCase("Honda", "Civic", "XYZ9876", 2023, "", 120000, "Color is required")]
    [TestCase("Honda", "Civic", "XYZ9876", 1903, "White", 120000, "Year must be a valid year")]
    [TestCase("Honda", "Civic", "XYZ9876", 2023, "White", 0, "Price must be greater than zero")]
    public async Task Update_WhenRequestIsInvalid_ReturnsFailure(
        string brand, string model, string plate, int year, string color, decimal price, string error)
    {
        var response = await new UpdateVehicleHandler(_repository.Object).Handle(
            new UpdateVehicleCommand(Guid.NewGuid(), brand, model, plate, year, color, price),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Reasons[0].Message, Is.EqualTo(error));
        });
        _repository.VerifyNoOtherCalls();
    }

    [TestCase(VehicleStatus.Available, "Reserve", VehicleStatus.Reserved)]
    [TestCase(VehicleStatus.Reserved, "Reserve", VehicleStatus.Reserved)]
    [TestCase(VehicleStatus.Reserved, "Sell", VehicleStatus.Sold)]
    [TestCase(VehicleStatus.Sold, "Sell", VehicleStatus.Sold)]
    [TestCase(VehicleStatus.Sold, "Deliver", VehicleStatus.Delivered)]
    [TestCase(VehicleStatus.Delivered, "Deliver", VehicleStatus.Delivered)]
    [TestCase(VehicleStatus.Available, "Release", VehicleStatus.Available)]
    [TestCase(VehicleStatus.Reserved, "Release", VehicleStatus.Available)]
    public async Task StateTransition_WhenAllowed_UpdatesVehicle(
        VehicleStatus initialStatus, string operation, VehicleStatus expectedStatus)
    {
        var id = Guid.NewGuid();
        var vehicle = ApplicationTestData.CreateVehicle(initialStatus);
        _repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);
        _repository.Setup(x => x.UpdateAsync(vehicle, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);

        var response = operation switch
        {
            "Reserve" => await new UpdateVehicleHandler(_repository.Object).Handle(new ReserveVehicleCommand(id), CancellationToken.None),
            "Sell" => await new UpdateVehicleHandler(_repository.Object).Handle(new SellVehicleCommand(id), CancellationToken.None),
            "Deliver" => await new UpdateVehicleHandler(_repository.Object).Handle(new DeliverVehicleCommand(id), CancellationToken.None),
            _ => await new UpdateVehicleHandler(_repository.Object).Handle(new ReleaseVehicleCommand(id), CancellationToken.None)
        };

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(vehicle.Status, Is.EqualTo(expectedStatus));
        });
        _repository.Verify(x => x.UpdateAsync(vehicle, It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase(VehicleStatus.Sold, "Reserve", "Is not possible to reserve a vehicle that is not available or reserved")]
    [TestCase(VehicleStatus.Available, "Sell", "Is not possible to sell a vehicle that is not reserved or sold")]
    [TestCase(VehicleStatus.Available, "Deliver", "Is not possible to deliver a vehicle that is not sold or delivered")]
    [TestCase(VehicleStatus.Sold, "Release", "Is not possible to release a vehicle that is not sold or delivered")]
    public async Task StateTransition_WhenNotAllowed_ReturnsFailure(
        VehicleStatus initialStatus, string operation, string error)
    {
        var id = Guid.NewGuid();
        var vehicle = ApplicationTestData.CreateVehicle(initialStatus);
        _repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);

        var response = operation switch
        {
            "Reserve" => await new UpdateVehicleHandler(_repository.Object).Handle(new ReserveVehicleCommand(id), CancellationToken.None),
            "Sell" => await new UpdateVehicleHandler(_repository.Object).Handle(new SellVehicleCommand(id), CancellationToken.None),
            "Deliver" => await new UpdateVehicleHandler(_repository.Object).Handle(new DeliverVehicleCommand(id), CancellationToken.None),
            _ => await new UpdateVehicleHandler(_repository.Object).Handle(new ReleaseVehicleCommand(id), CancellationToken.None)
        };

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Reasons[0].Message, Is.EqualTo(error));
        });
        _repository.Verify(x => x.UpdateAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task StateTransition_WhenVehicleDoesNotExist_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Vehicle?) null);

        var response = await new UpdateVehicleHandler(_repository.Object)
            .Handle(new ReserveVehicleCommand(id), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Reasons[0].Message, Is.EqualTo("Vehicle not found"));
        });
    }
}
