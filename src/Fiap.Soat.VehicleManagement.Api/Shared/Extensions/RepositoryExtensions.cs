using Fiap.Soat.VehicleManagement.Application.Adapters.Gateways.Repositories;
using Fiap.Soat.VehicleManagement.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.VehicleManagement.Api.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryExtensions(this IServiceCollection serviceCollection)
    {
        _ = serviceCollection.AddScoped<IVehicleRepository, VehicleRepository>();

        return serviceCollection;
    }
}
