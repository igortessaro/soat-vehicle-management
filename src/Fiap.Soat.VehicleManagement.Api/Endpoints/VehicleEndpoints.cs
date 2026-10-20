using Fiap.Soat.VehicleManagement.Api.Shared.Filters;
using Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;
using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.VehicleManagement.Api.Endpoints;

public static class VehicleEndpoints
{
    public static WebApplication Map(WebApplication app)
    {
        var vehiclesApi = app
            .MapGroup("api/v1/vehicles")
            .AddEndpointFilter<ResponseFilter<VehicleDetail>>()
            .WithTags("Vehicles");

        vehiclesApi.MapGet("/{id:guid}", ([FromServices] IVehicleController controller, Guid id, CancellationToken cancellationToken) =>
                    controller.GetAsync(id, cancellationToken))
            .WithName("GetVehicle")
            .WithDescription("Get a vehicle by ID");

        vehiclesApi.MapGet("", ([FromServices] IVehicleController controller, [AsParameters] ListVehicleQuery request, CancellationToken cancellationToken) =>
                controller.GetAllAsync(request, cancellationToken))
            .WithName("ListVehicles")
            .WithDescription("List all vehicles");

        vehiclesApi.MapPost("", ([FromServices] IVehicleController controller, [FromBody] CreateVehicleCommand command, CancellationToken cancellationToken) =>
                controller.CreateAsync(command, cancellationToken))
            .WithName("CreateVehicle")
            .WithDescription("Create a new vehicle");

        vehiclesApi.MapPut("/{id:guid}", ([FromServices] IVehicleController controller, Guid id, [FromBody] UpdateVehicleCommand command, CancellationToken cancellationToken) =>
                controller.UpdateAsync(id, command, cancellationToken))
            .WithName("UpdateVehicle")
            .WithDescription("Update an existing vehicle");

        return app;
    }
}
