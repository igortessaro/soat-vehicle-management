using Fiap.Soat.VehicleManagement.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<IReadOnlyList<Vehicle>> GetAllAsync(VehicleFilter filter, CancellationToken cancellationToken);
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken);
}
