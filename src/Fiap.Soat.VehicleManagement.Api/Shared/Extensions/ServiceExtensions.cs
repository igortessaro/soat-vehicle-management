using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.VehicleManagement.Api.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection serviceCollection)
    {
        // _ = serviceCollection.AddScoped<IAnalyzerFileService, AnalyzerFileService>();
        return serviceCollection;
    }
}
