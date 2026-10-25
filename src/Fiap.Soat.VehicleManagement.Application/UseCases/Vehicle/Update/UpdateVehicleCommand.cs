using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;
using System;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;

public record UpdateVehicleCommand(Guid Id, string Brand, string Model, string LicensePlate, int Year, string Color, decimal Price) : IRequest<Response<Domain.Entities.Vehicle>>;
