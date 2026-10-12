using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;

public sealed class UpdateVehicleHandler : IRequestHandler<UpdateVehicleCommand, Response<Domain.Entities.Vehicle>>
{
    public Task<Response<Domain.Entities.Vehicle>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ResponseFactory.Ok(new Domain.Entities.Vehicle()));
    }
}
