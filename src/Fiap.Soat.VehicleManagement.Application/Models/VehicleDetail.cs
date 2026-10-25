using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using System;

namespace Fiap.Soat.VehicleManagement.Application.Models;

public record VehicleDetail(Guid Id, string Brand, string Model, string LicensePlate, int Year, string Color, decimal Price, VehicleStatus Status, DateTime CreatedAt, DateTime UpdatedAt);
