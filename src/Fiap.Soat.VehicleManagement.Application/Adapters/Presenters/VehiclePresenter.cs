using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.Shared;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Presenters;

public static class VehiclePresenter
{
    public static VehicleDetail ToDto(Vehicle entity) =>
        new VehicleDetail(entity.Id, entity.Brand, entity.Model, entity.LicensePlate, entity.ManufactureYear, entity.Color, entity.Price, entity.Status, entity.CreatedAt, entity.UpdatedAt);

    public static Response<VehicleDetail> ToDto(Response<Vehicle> response) =>
        response.IsSuccess ?
            ResponseFactory.Ok(ToDto(response.Data)) :
            ResponseFactory.Fail<VehicleDetail>(response.Reasons[0].Message, response.StatusCode);

    public static IReadOnlyList<VehicleDetail> ToDto(IReadOnlyList<Vehicle> entities) => [.. entities.Select(ToDto)];

    public static Response<IReadOnlyList<VehicleDetail>> ToDto(Response<IReadOnlyList<Vehicle>> response) =>
        response.IsSuccess ?
            ResponseFactory.Ok(ToDto(response.Data)) :
            ResponseFactory.Fail<IReadOnlyList<VehicleDetail>>(response.Reasons[0].Message, response.StatusCode);
}
