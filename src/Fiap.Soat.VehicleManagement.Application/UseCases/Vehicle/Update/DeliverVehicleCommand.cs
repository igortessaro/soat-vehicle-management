using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;
using System;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;

public record DeliverVehicleCommand(Guid Id) : IRequest<Response<Domain.Entities.Vehicle>>;
