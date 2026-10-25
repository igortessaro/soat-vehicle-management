using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Moq;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Application.Tests;

public class CreateVehicleHandlerTests
{
    private readonly Mock<IVehicleRepository> _repository = new();

    [SetUp]
    public void SetUp() => _repository.Reset();

    [TestCase("", "Model", "ABC1234", 2024, "Black", 100000, "Brand is required")]
    [TestCase("Toyota", "", "ABC1234", 2024, "Black", 100000, "Model is required")]
    [TestCase("Toyota", "Corolla", "", 2024, "Black", 100000, "License plate is required")]
    [TestCase("Toyota", "Corolla", "ABC1234", 2024, "", 100000, "Color is required")]
    [TestCase("Toyota", "Corolla", "ABC1234", 1903, "Black", 100000, "Year must be a valid year")]
    [TestCase("Toyota", "Corolla", "ABC1234", 2024, "Black", 0, "Price must be greater than zero")]
    public async Task Handle_WhenRequestIsInvalid_ReturnsFailure(
        string brand, string model, string licensePlate, int year, string color, decimal price, string error)
    {
        var handler = new CreateVehicleHandler(_repository.Object);

        var response = await handler.Handle(
            new CreateVehicleCommand(brand, model, licensePlate, year, color, price),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Reasons[0].Message, Is.EqualTo(error));
        });
        _repository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Handle_WhenLicensePlateAlreadyExists_ReturnsFailure()
    {
        var existing = ApplicationTestData.CreateVehicle();
        _repository
            .Setup(x => x.GetByLicensePlateAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var response = await new CreateVehicleHandler(_repository.Object).Handle(
            new CreateVehicleCommand("Toyota", "Corolla", "ABC1234", 2024, "Black", 100000),
            CancellationToken.None);

        Assert.That(response.IsSuccess, Is.False);
        Assert.That(response.Reasons[0].Message, Is.EqualTo("Vehicle with this license plate already exists"));
        _repository.Verify(x => x.GetByLicensePlateAsync("ABC1234", It.IsAny<CancellationToken>()), Times.Once);
        _repository.VerifyNoOtherCalls();
    }

    [Test]
    public async Task Handle_WhenRequestIsValid_CreatesAndReturnsVehicle()
    {
        var created = ApplicationTestData.CreateVehicle();
        _repository
            .Setup(x => x.GetByLicensePlateAsync("ABC1234", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Vehicle?)null);
        _repository
            .Setup(x => x.AddAsync(It.IsAny<Vehicle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var response = await new CreateVehicleHandler(_repository.Object).Handle(
            new CreateVehicleCommand("Toyota", "Corolla", "ABC1234", 2024, "Black", 100000),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data, Is.SameAs(created));
        });
        _repository.Verify(x => x.AddAsync(
            It.Is<Vehicle>(v => v.Brand == "Toyota" && v.Price == 100000),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
