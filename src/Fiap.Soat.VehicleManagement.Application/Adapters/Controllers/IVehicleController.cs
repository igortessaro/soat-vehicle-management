using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;

public interface IVehicleController
{
    Task<IActionResult> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<IActionResult> GetAllAsync(ListVehicleQuery query, CancellationToken cancellationToken);
    Task<IActionResult> CreateAsync(CreateVehicleCommand request, CancellationToken cancellationToken);
    Task<IActionResult> UpdateAsync(Guid id, UpdateVehicleCommand request, CancellationToken cancellationToken);
    Task<IActionResult> ReserveAsync(Guid id, CancellationToken cancellationToken);
    Task<IActionResult> ReleaseAsync(Guid id, CancellationToken cancellationToken);
    Task<IActionResult> SellAsync(Guid id, CancellationToken cancellationToken);
}
