using System.Collections.Generic;

namespace Fiap.Soat.VehicleManagement.Domain.Shared;

public interface IPaginate<out T>
{
    IEnumerable<T> Items { get; }
    int TotalCount { get; }
    int PageSize { get; }
    int CurrentPage { get; }
    int TotalPages { get; }
}
