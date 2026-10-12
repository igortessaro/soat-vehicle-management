using Fiap.Soat.VehicleManagement.Application.Models;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;

public interface IVehicleController
{
    Task<VehicleDetail> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<VehicleDetail>> GetAllAsync(GetVehicleQuery query, CancellationToken cancellationToken);
    Task<VehicleDetail> CreateAsync(CreateVehicleCommand request, CancellationToken cancellationToken);
    Task<VehicleDetail> UpdateAsync(Guid id, UpdateVehicleCommand request, CancellationToken cancellationToken);
    Task<VehicleDetail> ReserveAsync(Guid id, CancellationToken cancellationToken);
    Task<VehicleDetail> ReleaseAsync(Guid id, CancellationToken cancellationToken);
    Task<VehicleDetail> SellAsync(Guid id, CancellationToken cancellationToken);
}
