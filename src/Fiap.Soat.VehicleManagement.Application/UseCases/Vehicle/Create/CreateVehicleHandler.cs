using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;

public sealed class CreateVehicleHandler : IRequestHandler<CreateVehicleCommand, Response<Domain.Entities.Vehicle>>
{
    public Task<Response<Domain.Entities.Vehicle>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ResponseFactory.Ok(new Domain.Entities.Vehicle()));
    }
}
