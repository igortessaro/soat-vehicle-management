using Fiap.Soat.VehicleManagement.Application.Adapters.Presenters;
using Fiap.Soat.VehicleManagement.Application.Mappers;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Get;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;

public sealed class VehicleController(IMediator mediator) : IVehicleController
{
    public async Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetVehicleByIdQuery(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public async Task<IActionResult> GetAllAsync(ListVehicleQuery query, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(query, cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public async Task<IActionResult> CreateAsync(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public async Task<IActionResult> UpdateAsync(Guid id, UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public async Task<IActionResult> ReserveAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ReserveVehicleCommand(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public async Task<IActionResult> ReleaseAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new ReleaseVehicleCommand(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public async Task<IActionResult> SellAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new SellVehicleCommand(id), cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }
}
