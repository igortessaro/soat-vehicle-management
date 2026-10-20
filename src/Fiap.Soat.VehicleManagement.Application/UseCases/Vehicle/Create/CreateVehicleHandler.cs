using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Application.Mappers;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;

public sealed class CreateVehicleHandler(IVehicleRepository repository) : IRequestHandler<CreateVehicleCommand, Response<Domain.Entities.Vehicle>>
{
    public async Task<Response<Domain.Entities.Vehicle>> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Brand)) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Brand is required");
        if (string.IsNullOrWhiteSpace(request.Model)) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Model is required");
        if (string.IsNullOrWhiteSpace(request.LicensePlate)) return ResponseFactory.Fail<Domain.Entities.Vehicle>("License plate is required");
        if (string.IsNullOrWhiteSpace(request.Color)) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Color is required");
        if (request.Year <= 1903) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Year must be a valid year");
        if (request.Price <= 0) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Price must be greater than zero");

        var existingVehicle = await repository.GetByLicensePlateAsync(request.LicensePlate, cancellationToken);
        if (existingVehicle != null) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Vehicle with this license plate already exists");

        var entity = request.ToDomainEntity();
        return ResponseFactory.Ok(await repository.AddAsync(entity, cancellationToken));
    }
}
