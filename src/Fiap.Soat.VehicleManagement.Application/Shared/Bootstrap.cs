using Fiap.Soat.VehicleManagement.Application.Adapters.Controllers;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.VehicleManagement.Application.Shared;

[ExcludeFromCodeCoverage]
public static class Bootstrap
{
    public static IServiceCollection AddInterfaceAdapters(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddScoped<IVehicleController, VehicleController>();
        return serviceCollection;
    }
}
