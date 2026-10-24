using Fiap.Soat.VehicleManagement.Api.Shared.Filters;
using Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;
using Fiap.Soat.VehicleManagement.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.VehicleManagement.Api.Endpoints;

public static class VehicleInternalEndpoints
{
    public static WebApplication Map(WebApplication app)
    {
        var vehiclesApi = app
            .MapGroup("api/v1/internal/vehicles")
            .AddEndpointFilter<ResponseFilter<VehicleDetail>>()
            .WithTags("Vehicles");

        vehiclesApi.MapPost("/{id:guid}/reserve", ([FromServices] IVehicleController controller, Guid id, CancellationToken cancellationToken) =>
            controller.ReserveAsync(id, cancellationToken))
            .WithName("ReserveVehicle")
            .WithDescription("Reserve a vehicle");

        vehiclesApi.MapPost("/{id:guid}/release", ([FromServices] IVehicleController controller, Guid id, CancellationToken cancellationToken) =>
            controller.ReleaseAsync(id, cancellationToken))
            .WithName("ReleaseVehicle")
            .WithDescription("Release a vehicle");

        vehiclesApi.MapPost("/{id:guid}/sell", ([FromServices] IVehicleController controller, Guid id, CancellationToken cancellationToken) =>
            controller.SellAsync(id, cancellationToken))
            .WithName("SellVehicle")
            .WithDescription("Sell a vehicle");

        vehiclesApi.MapPost("/{id:guid}/deliver", ([FromServices] IVehicleController controller, Guid id, CancellationToken cancellationToken) =>
            controller.DeliverAsync(id, cancellationToken))
            .WithName("DeliverVehicle")
            .WithDescription("Deliver a vehicle");

        return app;
    }
}
