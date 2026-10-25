using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;
using System;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;

public record ReleaseVehicleCommand(Guid Id) : IRequest<Response<Domain.Entities.Vehicle>>;
