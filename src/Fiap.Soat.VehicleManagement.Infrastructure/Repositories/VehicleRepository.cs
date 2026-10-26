using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.Soat.VehicleManagement.Infrastructure.Repositories;

public sealed class VehicleRepository(AppDbContext appDbContext) : Repository<Vehicle>(appDbContext), IVehicleRepository
{
    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(VehicleFilter filter, CancellationToken cancellationToken)
    {
        var query = Query();
        if (!string.IsNullOrEmpty(filter.LicensePlate)) query = query.Where(x => x.LicensePlate == filter.LicensePlate);
        if (!string.IsNullOrEmpty(filter.Model)) query = query.Where(x => x.Model == filter.Model);
        if (!string.IsNullOrEmpty(filter.Brand)) query = query.Where(x => x.Brand == filter.Brand);
        if (!string.IsNullOrEmpty(filter.Color)) query = query.Where(x => x.Color == filter.Color);
        if (filter.Year.HasValue) query = query.Where(x => x.ManufactureYear == filter.Year);
        if (filter.Status is { Length: > 0 } status) query = query.Where(x => Enumerable.Contains(status, x.Status));
        if (filter.UpdatedSince is { } updatedSince) query = query.Where(x => x.UpdatedAt >= updatedSince);
        return await query.OrderBy(x => x.Price).ToListAsync(cancellationToken);
    }

    public Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken) =>
        Query().FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
}
