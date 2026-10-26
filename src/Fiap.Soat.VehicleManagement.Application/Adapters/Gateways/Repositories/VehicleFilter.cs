using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using System;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;

public sealed record VehicleFilter(
    string? LicensePlate = null,
    string? Brand = null,
    string? Model = null,
    int? Year = null,
    string? Color = null,
    VehicleStatus[]? Status = null,
    DateTime? UpdatedSince = null);
