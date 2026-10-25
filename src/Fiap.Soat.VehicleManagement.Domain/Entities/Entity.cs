using System;

namespace Fiap.Soat.VehicleManagement.Domain.Entities;

public class Entity
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
}
