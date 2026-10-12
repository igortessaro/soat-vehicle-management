using Fiap.Soat.VehicleManagement.Application.Models;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;

public sealed class VehicleController : IVehicleController
{
    public Task<VehicleDetail> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<VehicleDetail>> GetAllAsync(GetVehicleQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<VehicleDetail> CreateAsync(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<VehicleDetail> UpdateAsync(Guid id, UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<VehicleDetail> ReserveAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<VehicleDetail> ReleaseAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<VehicleDetail> SellAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
