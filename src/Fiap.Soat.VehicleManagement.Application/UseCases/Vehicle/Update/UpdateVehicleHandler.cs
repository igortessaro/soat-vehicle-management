using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;

public sealed class UpdateVehicleHandler :
    IRequestHandler<UpdateVehicleCommand, Response<Domain.Entities.Vehicle>>,
    IRequestHandler<ReleaseVehicleCommand, Response<Domain.Entities.Vehicle>>,
    IRequestHandler<ReserveVehicleCommand, Response<Domain.Entities.Vehicle>>,
    IRequestHandler<SellVehicleCommand, Response<Domain.Entities.Vehicle>>
{
    public Task<Response<Domain.Entities.Vehicle>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ResponseFactory.Ok(new Domain.Entities.Vehicle()));
    }

    public Task<Response<Domain.Entities.Vehicle>> Handle(ReleaseVehicleCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ResponseFactory.Ok(new Domain.Entities.Vehicle()));
    }

    public Task<Response<Domain.Entities.Vehicle>> Handle(ReserveVehicleCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ResponseFactory.Ok(new Domain.Entities.Vehicle()));
    }

    public Task<Response<Domain.Entities.Vehicle>> Handle(SellVehicleCommand request, CancellationToken cancellationToken)
    {
        return Task.FromResult(ResponseFactory.Ok(new Domain.Entities.Vehicle()));
    }
}
