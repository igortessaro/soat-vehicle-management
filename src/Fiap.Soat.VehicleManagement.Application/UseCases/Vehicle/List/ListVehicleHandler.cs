using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;

public sealed class ListVehicleHandler : IRequestHandler<ListVehicleQuery, Response<IReadOnlyList<Domain.Entities.Vehicle>>>
{
    public Task<Response<IReadOnlyList<Domain.Entities.Vehicle>>> Handle(ListVehicleQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<Domain.Entities.Vehicle> vehicles = new List<Domain.Entities.Vehicle>();
        return Task.FromResult(ResponseFactory.Ok(vehicles));
    }
}
