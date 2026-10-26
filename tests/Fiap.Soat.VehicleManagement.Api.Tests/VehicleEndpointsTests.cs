using Fiap.Soat.VehicleManagement.Api.Endpoints;
using Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;
using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace Fiap.Soat.VehicleManagement.Api.Tests;

public class VehicleEndpointsTests
{
    private readonly Mock<IVehicleController> _controller = new();

    [SetUp]
    public void SetUp() => _controller.Reset();

    [Test]
    public async Task GetVehicle_PassesRouteIdAndReturnsVehicle()
    {
        var vehicle = CreateVehicleDetail();
        _controller.Setup(x => x.GetAsync(vehicle.Id, It.IsAny<CancellationToken>())).ReturnsAsync(ResponseFactory.Ok(vehicle));
        await using var server = await StartServer();

        var response = await server.Client.GetAsync($"api/v1/vehicles/{vehicle.Id}");

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("Corolla"));
        });
    }

    [Test]
    public async Task GetVehicle_WhenControllerFails_ReturnsBadRequestWithErrors()
    {
        var id = Guid.NewGuid();
        _controller.Setup(x => x.GetAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Fail<VehicleDetail>("Vehicle not found", HttpStatusCode.NotFound));
        await using var server = await StartServer();

        var response = await server.Client.GetAsync($"api/v1/vehicles/{id}");

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("Vehicle not found"));
        });
    }

    [Test]
    public async Task ListVehicles_BindsQueryStringFilters()
    {
        SetupList();
        await using var server = await StartServer();

        var response = await server.Client.GetAsync(
            "api/v1/vehicles?LicensePlate=ABC1234&Brand=Toyota&Model=Corolla&Year=2024&Color=Black&Status=Available&Status=Reserved");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        _controller.Verify(x => x.GetAllAsync(
            It.Is<ListVehicleQuery>(q =>
                q.LicensePlate == "ABC1234" && q.Brand == "Toyota" && q.Model == "Corolla" &&
                q.Year == 2024 && q.Color == "Black" &&
                q.Status.SequenceEqual(new[] { VehicleStatus.Available, VehicleStatus.Reserved })),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase("sold", new[] { VehicleStatus.Sold, VehicleStatus.Delivered })]
    [TestCase("available", new[] { VehicleStatus.Available })]
    public async Task ListByStatus_OverridesStatusFilter(string route, VehicleStatus[] expectedStatuses)
    {
        SetupList();
        await using var server = await StartServer();

        var response = await server.Client.GetAsync($"api/v1/vehicles/{route}?Brand=Toyota&Status=Reserved");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        _controller.Verify(x => x.GetAllAsync(
            It.Is<ListVehicleQuery>(q => q.Brand == "Toyota" && q.Status.SequenceEqual(expectedStatuses)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestCase("")]
    [TestCase("/sold")]
    [TestCase("/available")]
    public async Task ListVehicles_WhenControllerFails_ReturnsBadRequestWithErrors(string route)
    {
        _controller.Setup(x => x.GetAllAsync(It.IsAny<ListVehicleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Fail<IReadOnlyList<VehicleDetail>>("Invalid filter"));
        await using var server = await StartServer();

        var response = await server.Client.GetAsync($"api/v1/vehicles{route}");

        await Assert.MultipleAsync(async () =>
        {
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(await response.Content.ReadAsStringAsync(), Does.Contain("Invalid filter"));
        });
    }

    [Test]
    public async Task CreateVehicle_PassesBodyToController()
    {
        _controller.Setup(x => x.CreateAsync(It.IsAny<CreateVehicleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(CreateVehicleDetail(), HttpStatusCode.Created));
        await using var server = await StartServer();

        var response = await server.Client.PostAsJsonAsync("api/v1/vehicles",
            new { brand = "Toyota", model = "Corolla", licensePlate = "ABC1234", year = 2024, color = "Black", price = 100000 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        _controller.Verify(x => x.CreateAsync(
            new CreateVehicleCommand("Toyota", "Corolla", "ABC1234", 2024, "Black", 100000),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateVehicle_PassesRouteIdAndBodyToController()
    {
        var id = Guid.NewGuid();
        _controller.Setup(x => x.UpdateAsync(id, It.IsAny<UpdateVehicleCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok(CreateVehicleDetail()));
        await using var server = await StartServer();

        var response = await server.Client.PutAsJsonAsync($"api/v1/vehicles/{id}",
            new { brand = "Honda", model = "Civic", licensePlate = "XYZ9876", year = 2023, color = "White", price = 120000 });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        _controller.Verify(x => x.UpdateAsync(
            id,
            It.Is<UpdateVehicleCommand>(c => c.Brand == "Honda" && c.Price == 120000),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupList() =>
        _controller.Setup(x => x.GetAllAsync(It.IsAny<ListVehicleQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResponseFactory.Ok<IReadOnlyList<VehicleDetail>>([CreateVehicleDetail()]));

    private Task<EndpointTestServer> StartServer() =>
        EndpointTestServer.StartAsync(_controller.Object, VehicleEndpoints.Map);

    internal static VehicleDetail CreateVehicleDetail() =>
        new(Guid.NewGuid(), "Toyota", "Corolla", "ABC1234", 2024, "Black", 100000m, VehicleStatus.Available, DateTime.UtcNow, DateTime.UtcNow);
}
