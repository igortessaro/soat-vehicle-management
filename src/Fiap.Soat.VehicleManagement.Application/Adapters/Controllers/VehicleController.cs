using Fiap.Soat.VehicleManagement.Application.Adapters.Presenters;
using Fiap.Soat.VehicleManagement.Application.Mappers;
using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Get;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;

public sealed class VehicleController(IMediator mediator) : IVehicleController
{
    public async Task<Response<VehicleDetail>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetVehicleByIdQuery(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return result;
    }

    public async Task<Response<IReadOnlyList<VehicleDetail>>> GetAllAsync(ListVehicleQuery query, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return result;
    }

    public async Task<Response<VehicleDetail>> CreateAsync(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return result;
    }

    public async Task<Response<VehicleDetail>> UpdateAsync(Guid id, UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request with { Id = id }, cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return result;
    }

    public async Task<Response<VehicleDetail>> ReserveAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ReserveVehicleCommand(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return result;
    }

    public async Task<Response<VehicleDetail>> ReleaseAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ReleaseVehicleCommand(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return result;
    }

    public async Task<Response<VehicleDetail>> SellAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new SellVehicleCommand(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return result;
    }

    public async Task<Response<VehicleDetail>> DeliverAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new DeliverVehicleCommand(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return result;
    }
}
