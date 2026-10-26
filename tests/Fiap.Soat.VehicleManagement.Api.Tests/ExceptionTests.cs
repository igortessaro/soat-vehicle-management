using Fiap.Soat.VehicleManagement.Api.Shared.Exceptions;

namespace Fiap.Soat.VehicleManagement.Api.Tests;

public class ExceptionTests
{
    [Test]
    public void DomainException_WithInnerException_PreservesDetails()
    {
        var inner = new InvalidOperationException("inner");
        var exception = new DomainException("domain", inner);

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo("domain"));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        });
    }

    [Test]
    public void ResourceNotFoundException_WithMessage_StoresMessage()
    {
        var exception = new ResourceNotFoundException("vehicle not found");

        Assert.That(exception.Message, Is.EqualTo("vehicle not found"));
    }

    [Test]
    public void ResourceNotFoundException_WithInnerException_PreservesDetails()
    {
        var inner = new InvalidOperationException("inner");
        var exception = new ResourceNotFoundException("vehicle not found", inner);

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message, Is.EqualTo("vehicle not found"));
            Assert.That(exception.InnerException, Is.SameAs(inner));
        });
    }
}
