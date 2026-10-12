using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Get;

public record GetVehicleByIdQuery(Guid Id) : IRequest<Response<Domain.Entities.Vehicle>>;
