using Fiap.Soat.VehicleManagement.Application.Adapters.Presenters;
using Fiap.Soat.VehicleManagement.Application.Mappers;
using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;

public sealed class VehicleController(IMediator mediator) : IVehicleController
{
    public Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> GetAllAsync(GetVehicleQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IActionResult> CreateAsync(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        var response = await mediator.Send(request, cancellationToken);
        var result = ResponseMapper.Map(response, VehiclePresenter.ToDto);
        return ActionResultPresenter.ToActionResult(result);
    }

    public Task<IActionResult> UpdateAsync(Guid id, UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> ReserveAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> ReleaseAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IActionResult> SellAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
