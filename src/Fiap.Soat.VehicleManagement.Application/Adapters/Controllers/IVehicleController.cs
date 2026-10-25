using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.List;
using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Update;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;

public interface IVehicleController
{
    Task<Response<VehicleDetail>> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<Response<IReadOnlyList<VehicleDetail>>> GetAllAsync(ListVehicleQuery query, CancellationToken cancellationToken);
    Task<Response<VehicleDetail>> CreateAsync(CreateVehicleCommand request, CancellationToken cancellationToken);
    Task<Response<VehicleDetail>> UpdateAsync(Guid id, UpdateVehicleCommand request, CancellationToken cancellationToken);
    Task<Response<VehicleDetail>> ReserveAsync(Guid id, CancellationToken cancellationToken);
    Task<Response<VehicleDetail>> ReleaseAsync(Guid id, CancellationToken cancellationToken);
    Task<Response<VehicleDetail>> SellAsync(Guid id, CancellationToken cancellationToken);
    Task<Response<VehicleDetail>> DeliverAsync(Guid id, CancellationToken cancellationToken);
}
