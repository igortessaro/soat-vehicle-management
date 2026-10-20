using System.Text.RegularExpressions;

namespace Fiap.Soat.VehicleManagement.Domain.ValueObjects;

public record LicensePlate
{
    private LicensePlate() { }

    private LicensePlate(string licensePlate)
    {
        Value = string.IsNullOrWhiteSpace(licensePlate) ? string.Empty : licensePlate.Trim().ToUpper();
    }

    public string Value { get; private set; } = string.Empty;

    public bool IsValid()
    {
        if (string.IsNullOrWhiteSpace(Value)) return false;

        var oldPattern = new Regex(@"^[A-Z]{3}-?[0-9]{4}$", RegexOptions.NonBacktracking);
        var newMercosulPattern = new Regex(@"^[A-Z]{3}[0-9][A-Z][0-9]{2}$", RegexOptions.NonBacktracking);

        return oldPattern.IsMatch(Value) || newMercosulPattern.IsMatch(Value);
    }

    public static implicit operator LicensePlate(string value) => new LicensePlate(value);
    public static implicit operator string(LicensePlate licensePlate) => licensePlate.Value;
}
