using Fiap.Soat.VehicleManagement.Api.Endpoints;
using Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;
using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Api.Tests;

public class VehicleInternalEndpointsTests
{
    private readonly Mock<IVehicleController> _controller = new();

    [SetUp]
    public void SetUp() => _controller.Reset();

    [TestCase("reserve")]
    [TestCase("release")]
    [TestCase("sell")]
    [TestCase("deliver")]
    public async Task StateOperation_PassesRouteIdToController(string operation)
    {
        var id = Guid.NewGuid();
        _controller.Setup(OperationCall(operation, id))
            .ReturnsAsync(ResponseFactory.Ok(VehicleEndpointsTests.CreateVehicleDetail()));
        await using var server = await StartServer();

        var response = await server.Client.PostAsync($"api/v1/internal/vehicles/{id}/{operation}", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        _controller.Verify(OperationCall(operation, id), Times.Once);
    }

    [TestCase("reserve")]
    [TestCase("release")]
    [TestCase("sell")]
    [TestCase("deliver")]
    public async Task StateOperation_WhenControllerFails_ReturnsBadRequestWithErrors(string operation)
    {
        var id = Guid.NewGuid();
        _controller.Setup(OperationCall(operation, id))
            .ReturnsAsync(ResponseFactory.Fail<VehicleDetail>("Transition not allowed"));
        await using var server = await StartServer();

        var response = await server.Client.PostAsync($"api/v1/internal/vehicles/{id}/{operation}", null);

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("Transition not allowed"));
        });
    }

    private static Expression<Func<IVehicleController, Task<Response<VehicleDetail>>>> OperationCall(string operation, Guid id) =>
        operation switch
        {
            "reserve" => x => x.ReserveAsync(id, It.IsAny<CancellationToken>()),
            "release" => x => x.ReleaseAsync(id, It.IsAny<CancellationToken>()),
            "sell" => x => x.SellAsync(id, It.IsAny<CancellationToken>()),
            _ => x => x.DeliverAsync(id, It.IsAny<CancellationToken>())
        };

    private Task<EndpointTestServer> StartServer() =>
        EndpointTestServer.StartAsync(_controller.Object, VehicleInternalEndpoints.Map);
}
