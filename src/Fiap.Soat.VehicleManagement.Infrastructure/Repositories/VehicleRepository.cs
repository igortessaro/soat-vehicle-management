using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Soat.VehicleManagement.Infrastructure.Repositories;

public sealed class VehicleRepository(AppDbContext appDbContext) : Repository<Vehicle>(appDbContext), IVehicleRepository
{
    public async Task<IReadOnlyList<Vehicle>> GetAllAsync(
        string? licensePlate,
        string? model,
        string? brand,
        string? color,
        int? year,
        string? status,
        CancellationToken cancellationToken)
    {
        var query = Query();
        if (!string.IsNullOrEmpty(licensePlate)) query = query.Where(x => x.LicensePlate == licensePlate);
        if (!string.IsNullOrEmpty(model)) query = query.Where(x => x.Model == model);
        if (!string.IsNullOrEmpty(brand)) query = query.Where(x => x.Brand == brand);
        if (!string.IsNullOrEmpty(color)) query = query.Where(x => x.Color == color);
        if (year.HasValue) query = query.Where(x => x.Year == year);
        if (!string.IsNullOrEmpty(status)) query = query.Where(x => x.Status == status);
        return await query.ToListAsync(cancellationToken);
    }
}
