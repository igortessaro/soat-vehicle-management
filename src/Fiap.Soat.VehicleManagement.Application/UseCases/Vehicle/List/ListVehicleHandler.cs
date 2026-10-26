using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;

public sealed class ListVehicleHandler(IVehicleRepository repository) : IRequestHandler<ListVehicleQuery, Response<IReadOnlyList<Domain.Entities.Vehicle>>>
{
    public async Task<Response<IReadOnlyList<Domain.Entities.Vehicle>>> Handle(ListVehicleQuery request, CancellationToken cancellationToken) =>
        ResponseFactory.Ok(await repository.GetAllAsync(
            new VehicleFilter(
                request.LicensePlate,
                request.Brand,
                request.Model,
                request.Year,
                request.Color,
                request.Status,
                request.UpdatedSince),
            cancellationToken
        ));
}
