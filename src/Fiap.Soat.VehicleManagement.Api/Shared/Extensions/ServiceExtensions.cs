using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.VehicleManagement.Api.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection serviceCollection)
    {
        return serviceCollection;
    }
}
