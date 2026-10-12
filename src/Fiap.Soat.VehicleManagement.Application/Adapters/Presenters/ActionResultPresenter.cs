using Fiap.Soat.VehicleManagement.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Soat.VehicleManagement.Application.Adapters.Presenters;

public static class ActionResultPresenter
{
    public static ActionResult ToActionResult(Response result) => new ObjectResult(result) { StatusCode = (int?) result.StatusCode };

    public static ActionResult ToActionResult<T>(Response<T> result) => new ObjectResult(result) { StatusCode = (int?) result.StatusCode };
}
