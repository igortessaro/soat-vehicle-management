using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<IReadOnlyList<Vehicle>> GetAllAsync(string? licensePlate, string? model, string? brand, string? color, int? year, VehicleStatus[] status, CancellationToken cancellationToken);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken);
}
