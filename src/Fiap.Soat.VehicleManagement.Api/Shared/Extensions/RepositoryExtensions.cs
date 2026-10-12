using System.Diagnostics.CodeAnalysis;

namespace Fiap.Soat.VehicleManagement.Api.Shared.Extensions;

[ExcludeFromCodeCoverage]
public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositoryExtensions(this IServiceCollection serviceCollection)
    {
        // _ = serviceCollection.AddScoped<IProcessingEventLogRepository, ProcessingEventLogRepository>();
        // _ = serviceCollection.AddScoped<IProcessingFileRepository, ProcessingFileRepository>();

        return serviceCollection;
    }
}
