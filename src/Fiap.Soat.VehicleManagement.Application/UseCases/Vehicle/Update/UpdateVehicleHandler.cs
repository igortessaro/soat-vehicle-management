using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using MediatR;

namespace Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;

public sealed class UpdateVehicleHandler(IVehicleRepository repository) :
    IRequestHandler<UpdateVehicleCommand, Response<Domain.Entities.Vehicle>>,
    IRequestHandler<ReleaseVehicleCommand, Response<Domain.Entities.Vehicle>>,
    IRequestHandler<ReserveVehicleCommand, Response<Domain.Entities.Vehicle>>,
    IRequestHandler<SellVehicleCommand, Response<Domain.Entities.Vehicle>>
{
    public async Task<Response<Domain.Entities.Vehicle>> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Brand)) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Brand is required");
        if (string.IsNullOrWhiteSpace(request.Model)) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Model is required");
        if (string.IsNullOrWhiteSpace(request.LicensePlate)) return ResponseFactory.Fail<Domain.Entities.Vehicle>("License plate is required");
        if (string.IsNullOrWhiteSpace(request.Color)) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Color is required");
        if (request.Year > 1903) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Year must be a valid year");
        if (request.Price <= 0) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Price must be greater than zero");

        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Vehicle not found");
        entity.Update(request.Brand, request.Model, request.LicensePlate, request.Year, request.Color, request.Price);
        await repository.UpdateAsync(entity, cancellationToken);
        return ResponseFactory.Ok(entity);
    }

    public async Task<Response<Domain.Entities.Vehicle>> Handle(ReleaseVehicleCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Vehicle not found");
        if (entity.Status != VehicleStatus.Sold && entity.Status != VehicleStatus.Delivered)
        {
            return ResponseFactory.Fail<Domain.Entities.Vehicle>("Is not possible to release a vehicle that is not sold or delivered");
        }
        await repository.UpdateAsync(entity.Release(), cancellationToken);
        return ResponseFactory.Ok(entity);
    }

    public async Task<Response<Domain.Entities.Vehicle>> Handle(ReserveVehicleCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Vehicle not found");
        if (entity.Status != VehicleStatus.Available && entity.Status != VehicleStatus.Reserved)
        {
            return ResponseFactory.Fail<Domain.Entities.Vehicle>("Is not possible to reserve a vehicle that is not available or reserved");
        }
        await repository.UpdateAsync(entity.Reserve(), cancellationToken);
        return ResponseFactory.Ok(entity);
    }

    public async Task<Response<Domain.Entities.Vehicle>> Handle(SellVehicleCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken);
        if (entity == null) return ResponseFactory.Fail<Domain.Entities.Vehicle>("Vehicle not found");
        if (entity.Status != VehicleStatus.Reserved && entity.Status != VehicleStatus.Sold)
        {
            return ResponseFactory.Fail<Domain.Entities.Vehicle>("Is not possible to sell a vehicle that is not reserved or sold");
        }
        await repository.UpdateAsync(entity.Sold(), cancellationToken);
        return ResponseFactory.Ok(entity);
    }
}
