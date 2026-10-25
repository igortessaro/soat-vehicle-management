using Fiap.Soat.VehicleManagement.Domain.ValueObjects;

namespace Fiap.Soat.VehicleManagement.Domain.Tests;

public class LicensePlateTests
{
    [TestCase("abc1d23", "ABC1D23")]
    [TestCase(" abc-1234 ", "ABC-1234")]
    public void ImplicitConversion_ShouldNormalizeValue(string input, string expectedValue)
    {
        LicensePlate plate = input;

        Assert.That(plate.Value, Is.EqualTo(expectedValue));
    }

    [TestCase("ABC1234")]
    [TestCase("ABC-1234")]
    [TestCase("ABC1D23")]
    public void IsValid_ShouldReturnTrueForSupportedFormats(string value)
    {
        LicensePlate plate = value;

        Assert.That(plate.IsValid(), Is.True);
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("AB12345")]
    [TestCase("ABC12D3")]
    [TestCase("ABC12345")]
    public void IsValid_ShouldReturnFalseForUnsupportedFormats(string value)
    {
        LicensePlate plate = value;

        Assert.That(plate.IsValid(), Is.False);
    }

    [Test]
    public void ImplicitStringConversion_ShouldReturnNormalizedValue()
    {
        LicensePlate plate = "abc1d23";
        string value = plate;

        Assert.That(value, Is.EqualTo("ABC1D23"));
    }
}
