using Fiap.Soat.VehicleManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Soat.VehicleManagement.Infrastructure.Tests;

internal static class InfrastructureTestFixture
{
    public static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
