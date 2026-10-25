using Fiap.Soat.VehicleManagement.Domain.Shared;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using MediatR;
using System.Collections.Generic;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;

public record ListVehicleQuery(string? LicensePlate, string? Brand, string? Model, int? Year, string? Color, VehicleStatus[] Status)
    : IRequest<Response<IReadOnlyList<Domain.Entities.Vehicle>>>;
