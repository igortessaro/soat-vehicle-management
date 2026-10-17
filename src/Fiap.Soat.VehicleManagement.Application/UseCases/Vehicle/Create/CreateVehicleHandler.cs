using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Application.Mappers;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;

public sealed class CreateVehicleHandler(IVehicleRepository repository) : IRequestHandler<CreateVehicleCommand, Response<Domain.Entities.Vehicle>>
{
    public async Task<Response<Domain.Entities.Vehicle>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var entity = request.ToDomainEntity();
        return ResponseFactory.Ok(await repository.AddAsync(entity, cancellationToken));
    }
}
