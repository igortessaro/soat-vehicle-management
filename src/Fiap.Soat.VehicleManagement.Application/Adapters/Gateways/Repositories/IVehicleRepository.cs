using Fiap.Soat.VehicleManagement.Domain.Entities;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<IReadOnlyList<Vehicle>> GetAllAsync(string? licensePlate, string? model, string? brand, string? color, int? year, string? status, CancellationToken cancellationToken);
}
