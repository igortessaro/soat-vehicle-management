using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;

namespace Fiap.Soat.VehicleManagement.Api.Tests;

internal static class ApiTestData
{
    public static VehicleDetail CreateVehicleDetail() =>
        new(Guid.NewGuid(), "Toyota", "Corolla", "ABC1234", 2024, "Black", 100000m, VehicleStatus.Available, DateTime.UtcNow, DateTime.UtcNow);
}
