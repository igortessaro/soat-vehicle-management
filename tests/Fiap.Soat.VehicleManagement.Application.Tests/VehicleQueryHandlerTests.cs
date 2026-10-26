using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Get;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Moq;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Application.Tests;

public class VehicleQueryHandlerTests
{
    private readonly Mock<IVehicleRepository> _repository = new();

    [SetUp]
    public void SetUp() => _repository.Reset();

    [Test]
    public async Task GetById_WhenVehicleExists_ReturnsVehicle()
    {
        var vehicle = ApplicationTestData.CreateVehicle();
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(vehicle);

        var response = await new GetVehicleByIdHandler(_repository.Object)
            .Handle(new GetVehicleByIdQuery(id), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data, Is.SameAs(vehicle));
        });
    }

    [Test]
    public async Task GetById_WhenVehicleDoesNotExist_ReturnsFailure()
    {
        var id = Guid.NewGuid();
        _repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Vehicle?) null);

        var response = await new GetVehicleByIdHandler(_repository.Object)
            .Handle(new GetVehicleByIdQuery(id), CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Reasons[0].Message, Does.Contain(id.ToString()));
        });
    }

    [Test]
    public async Task List_PassesAllFiltersToRepository()
    {
        var vehicles = new[] { ApplicationTestData.CreateVehicle() };
        var statuses = new[] { VehicleStatus.Available, VehicleStatus.Reserved };
        _repository
            .Setup(x => x.GetAllAsync(
                new VehicleFilter("ABC1234", "Toyota", "Corolla", 2024, "Black", statuses), It.IsAny<CancellationToken>()))
            .ReturnsAsync(vehicles);

        var response = await new ListVehicleHandler(_repository.Object).Handle(
            new ListVehicleQuery("ABC1234", "Toyota", "Corolla", 2024, "Black", statuses, null),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.Data, Is.SameAs(vehicles));
        });
        _repository.Verify(x => x.GetAllAsync(
            new VehicleFilter("ABC1234", "Toyota", "Corolla", 2024, "Black", statuses), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task List_WhenUpdatedSinceIsInformed_ForwardsItToRepository()
    {
        var updatedSince = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        var statuses = Array.Empty<VehicleStatus>();
        var expectedFilter = new VehicleFilter(Status: statuses, UpdatedSince: updatedSince);
        using var cancellation = new CancellationTokenSource();
        _repository
            .Setup(x => x.GetAllAsync(expectedFilter, cancellation.Token))
            .ReturnsAsync([]);

        var response = await new ListVehicleHandler(_repository.Object).Handle(
            new ListVehicleQuery(null, null, null, null, null, statuses, updatedSince),
            cancellation.Token);

        Assert.That(response.IsSuccess, Is.True);
        _repository.Verify(x => x.GetAllAsync(expectedFilter, cancellation.Token), Times.Once);
    }
}
