using Fiap.Soat.VehicleManagement.Api.Shared.Filters;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using Microsoft.AspNetCore.Http;

namespace Fiap.Soat.VehicleManagement.Api.Tests;

public class ResponseFilterTests
{
    [Test]
    public async Task InvokeAsync_WhenTypedResponseFails_ReturnsBadRequestJson()
    {
        var filter = new ResponseFilter<string>();
        var context = new TestInvocationContext();

        var result = await filter.InvokeAsync(context, _ =>
            ValueTask.FromResult<object?>(ResponseFactory.Fail<string>("invalid")));

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task InvokeAsync_WhenResponseFails_ReturnsBadRequestJson()
    {
        var filter = new ResponseFilter<string>();
        var context = new TestInvocationContext();

        var result = await filter.InvokeAsync(context, _ =>
            ValueTask.FromResult<object?>(ResponseFactory.Fail("invalid")));

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task InvokeAsync_WhenResponseSucceeds_ReturnsOriginalResult()
    {
        var filter = new ResponseFilter<string>();
        var expected = ResponseFactory.Ok("ok");
        var context = new TestInvocationContext();

        var result = await filter.InvokeAsync(context, _ => ValueTask.FromResult<object?>(expected));

        Assert.That(result, Is.SameAs(expected));
    }

    private sealed class TestInvocationContext : EndpointFilterInvocationContext
    {
        public override HttpContext HttpContext { get; } = new DefaultHttpContext();
        public override IList<object?> Arguments { get; } = [];
    }
}
