using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Get;

public sealed class GetVehicleByIdHandler : IRequestHandler<GetVehicleByIdQuery, Response<Domain.Entities.Vehicle>>
{
    public Task<Response<Domain.Entities.Vehicle>> Handle(GetVehicleByIdQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ResponseFactory.Ok(new Domain.Entities.Vehicle()));
    }
}
