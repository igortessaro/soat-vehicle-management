using Fiap.Soat.VehicleManagement.Application.UseCases.Vehicle.Create;

namespace Fiap.Soat.VehicleManagement.Application.Mappers;

public static class VehicleMapper
{
    public static Domain.Entities.Vehicle ToDomainEntity(this CreateVehicleCommand command) =>
        new Domain.Entities.Vehicle(command.Brand, command.Model, command.LicensePlate, command.Year, command.Color).UpdatePrice(command.Price);
}
