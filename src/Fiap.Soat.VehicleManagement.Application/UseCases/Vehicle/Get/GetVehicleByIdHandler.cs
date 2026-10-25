using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Get;

public sealed class GetVehicleByIdHandler(IVehicleRepository repository) : IRequestHandler<GetVehicleByIdQuery, Response<Domain.Entities.Vehicle>>
{
    public Task<Response<Domain.Entities.Vehicle>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken) =>
        repository.GetByIdAsync(request.Id, cancellationToken)
            .ContinueWith(task => task.Result is null
                ? ResponseFactory.Fail<Domain.Entities.Vehicle>($"Vehicle with id {request.Id} not found")
                : ResponseFactory.Ok(task.Result), cancellationToken);
}
