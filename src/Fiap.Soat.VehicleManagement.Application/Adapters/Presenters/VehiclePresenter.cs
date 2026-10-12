using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Entities;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Presenters;

public static class VehiclePresenter
{
    public static VehicleDetail ToDto(Vehicle entity) =>
        new VehicleDetail(entity.Id, entity.Brand, entity.Model, entity.LicensePlate, entity.Year, entity.Color, entity.Price, entity.Status, entity.CreatedAt, entity.UpdatedAt);

    public static IReadOnlyList<VehicleDetail> ToDto(IReadOnlyList<Vehicle> entities) => [.. entities.Select(ToDto)];
}
