using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;
using System.Linq.Expressions;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;

public sealed class ListVehicleHandler(IVehicleRepository repository) : IRequestHandler<ListVehicleQuery, Response<IReadOnlyList<Domain.Entities.Vehicle>>>
{
    public async Task<Response<IReadOnlyList<Domain.Entities.Vehicle>>> Handle(ListVehicleQuery request, CancellationToken cancellationToken) =>
        ResponseFactory.Ok(await repository.GetAllAsync(
            request.LicensePlate,
            request.Model,
            request.Brand,
            request.Color,
            request.Year,
            request.Status,
            cancellationToken
        ));
}
