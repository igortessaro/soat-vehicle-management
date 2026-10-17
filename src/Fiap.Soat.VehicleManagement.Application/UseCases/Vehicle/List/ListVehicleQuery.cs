using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;

public record ListVehicleQuery(string? LicensePlate, string? Brand, string? Model, int? Year, string? Color, string? Status)
    : IRequest<Response<IReadOnlyList<Domain.Entities.Vehicle>>>;
