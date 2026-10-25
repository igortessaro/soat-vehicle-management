using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using Fiap.Soat.VehicleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.Soat.VehicleManagement.Infrastructure.Repositories;

public sealed class VehicleRepository(AppDbContext appDbContext) : Repository<Vehicle>(appDbContext), IVehicleRepository
{
    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(
        string? licensePlate,
        string? model,
        string? brand,
        string? color,
        int? year,
        VehicleStatus[] status,
        CancellationToken cancellationToken)
    {
        var query = Query();
        if (!string.IsNullOrEmpty(licensePlate)) query = query.Where(x => x.LicensePlate == licensePlate);
        if (!string.IsNullOrEmpty(model)) query = query.Where(x => x.Model == model);
        if (!string.IsNullOrEmpty(brand)) query = query.Where(x => x.Brand == brand);
        if (!string.IsNullOrEmpty(color)) query = query.Where(x => x.Color == color);
        if (year.HasValue) query = query.Where(x => x.ManufactureYear == year);
        if (status is { Length: > 0 }) query = query.Where(x => status.Contains(x.Status));
        return await query.OrderBy(x => x.Price).ToListAsync(cancellationToken);
    }

    public Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken cancellationToken) =>
        Query().FirstOrDefaultAsync(x => x.LicensePlate == licensePlate, cancellationToken);
}
