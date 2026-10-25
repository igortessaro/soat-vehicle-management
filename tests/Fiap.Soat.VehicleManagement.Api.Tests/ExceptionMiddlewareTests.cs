using Fiap.Soat.VehicleManagement.Api.Shared.Exceptions;
using Fiap.Soat.VehicleManagement.Api.Shared.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text;

namespace Fiap.Soat.VehicleManagement.Api.Tests;

public class ExceptionMiddlewareTests
{
    [TestCase(typeof(ArgumentNullException), 400)]
    [TestCase(typeof(BadHttpRequestException), 400)]
    [TestCase(typeof(DomainException), 400)]
    [TestCase(typeof(ResourceNotFoundException), 404)]
    [TestCase(typeof(KeyNotFoundException), 404)]
    [TestCase(typeof(UnauthorizedAccessException), 401)]
    [TestCase(typeof(InvalidOperationException), 500)]
    public async Task InvokeAsync_MapsExceptionToExpectedStatus(Type exceptionType, int expectedStatusCode)
    {
        var exception = (Exception) Activator.CreateInstance(exceptionType, "failure")!;
        var middleware = new ExceptionMiddleware(
            _ => throw exception,
            NullLogger<ExceptionMiddleware>.Instance);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.Multiple(() =>
        {
            Assert.That(context.Response.StatusCode, Is.EqualTo(expectedStatusCode));
            Assert.That(context.Response.ContentType, Is.EqualTo("application/json"));
        });
        var body = await ReadBody(context);
        Assert.That(body, Does.Contain("failure"));
    }

    [Test]
    public async Task InvokeAsync_WhenNextSucceeds_PreservesResponse()
    {
        var middleware = new ExceptionMiddleware(
            context => context.Response.WriteAsync("ok"),
            NullLogger<ExceptionMiddleware>.Instance);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        Assert.That(await ReadBody(context), Is.EqualTo("ok"));
    }

    [Test]
    public async Task InvokeAsync_ForBadRequest_ReturnsFailureResponseShape()
    {
        var middleware = new ExceptionMiddleware(
            _ => throw new DomainException("invalid vehicle"),
            NullLogger<ExceptionMiddleware>.Instance);
        var context = CreateContext();

        await middleware.InvokeAsync(context);

        var body = await ReadBody(context);
        Assert.That(body, Does.Contain("invalid vehicle"));
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(context.Response.Body, Encoding.UTF8);
        return await reader.ReadToEndAsync();
    }
}
