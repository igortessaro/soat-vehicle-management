using Fiap.Soat.VehicleManagement.Domain.Entities;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;

namespace Fiap.Soat.VehicleManagement.Application.Tests;

internal static class ApplicationTestData
{
    public static Vehicle CreateVehicle(VehicleStatus status = VehicleStatus.Available)
    {
        var vehicle = new Vehicle("Toyota", "Corolla", "ABC-1234", 2024, "Black")
            .UpdatePrice(100000);

        return status switch
        {
            VehicleStatus.Available => vehicle,
            VehicleStatus.Reserved => vehicle.Reserve(),
            VehicleStatus.Sold => vehicle.Reserve().Sold(),
            VehicleStatus.Delivered => vehicle.Reserve().Sold().Deliver(),
            _ => vehicle
        };
    }
}
