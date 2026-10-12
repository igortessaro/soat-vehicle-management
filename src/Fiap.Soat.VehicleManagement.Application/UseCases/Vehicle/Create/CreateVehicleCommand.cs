using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;

public record CreateVehicleCommand(string Brand, string Model, string LicensePlate, int Year, string Color, decimal Price, string Status)
    : IRequest<Response<Domain.Entities.Vehicle>>;
