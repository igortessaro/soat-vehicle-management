using Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Get;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;
using Moq;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Application.Tests;

public class VehicleControllerTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly VehicleController _controller;

    public VehicleControllerTests() => _controller = new VehicleController(_mediator.Object);

    [Test]
    public async Task GetAsync_SendsQueryAndMapsResponse()
    {
        var id = Guid.NewGuid();
        var vehicle = ApplicationTestData.CreateVehicle();
        _mediator.Setup(x => x.Send(It.IsAny<IRequest<Response<Domain.Entities.Vehicle>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(vehicle));

        var response = await _controller.GetAsync(id, CancellationToken.None);

        Assert.That(response.Data.Brand, Is.EqualTo(vehicle.Brand));
        _mediator.Verify(x => x.Send(It.Is<GetVehicleByIdQuery>(q => q.Id == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_SendsOriginalQuery()
    {
        var query = new ListVehicleQuery(null, "Toyota", null, null, null, [], null);
        _mediator.Setup(x => x.Send(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok<IReadOnlyList<Domain.Entities.Vehicle>>([]));

        var response = await _controller.GetAllAsync(query, CancellationToken.None);

        Assert.That(response.IsSuccess, Is.True);
        _mediator.Verify(x => x.Send(query, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateAsync_SendsOriginalCommand()
    {
        var command = new CreateVehicleCommand("Toyota", "Corolla", "ABC1234", 2024, "Black", 100000);
        _mediator.Setup(x => x.Send(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(ApplicationTestData.CreateVehicle(), HttpStatusCode.Created));

        var response = await _controller.CreateAsync(command, CancellationToken.None);

        Assert.That(response.IsSuccess, Is.True);
        _mediator.Verify(x => x.Send(command, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_ReplacesCommandId()
    {
        var id = Guid.NewGuid();
        var command = new UpdateVehicleCommand(Guid.Empty, "Toyota", "Corolla", "ABC1234", 2024, "Black", 100000);
        _mediator.Setup(x => x.Send(It.IsAny<IRequest<Response<Domain.Entities.Vehicle>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(ApplicationTestData.CreateVehicle()));

        await _controller.UpdateAsync(id, command, CancellationToken.None);

        _mediator.Verify(x => x.Send(It.Is<UpdateVehicleCommand>(c => c.Id == id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase("Reserve")]
    [TestCase("Release")]
    [TestCase("Sell")]
    [TestCase("Deliver")]
    public async Task StateOperation_SendsCommand(string operation)
    {
        var id = Guid.NewGuid();
        _mediator.Setup(x => x.Send(It.IsAny<IRequest<Response<Domain.Entities.Vehicle>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(ApplicationTestData.CreateVehicle()));

        _ = operation switch
        {
            "Reserve" => await _controller.ReserveAsync(id, CancellationToken.None),
            "Release" => await _controller.ReleaseAsync(id, CancellationToken.None),
            "Sell" => await _controller.SellAsync(id, CancellationToken.None),
            _ => await _controller.DeliverAsync(id, CancellationToken.None)
        };

        var expectedType = operation switch
        {
            "Reserve" => typeof(ReserveVehicleCommand),
            "Release" => typeof(ReleaseVehicleCommand),
            "Sell" => typeof(SellVehicleCommand),
            _ => typeof(DeliverVehicleCommand)
        };

        _mediator.Verify(x => x.Send(
            It.Is<IRequest<Response<Domain.Entities.Vehicle>>>(request => request.GetType() == expectedType),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
