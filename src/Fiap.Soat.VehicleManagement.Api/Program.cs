using Fiap.Soat.VehicleManagement.Api.Endpoints;
using Fiap.Soat.VehicleManagement.Api.Shared.Extensions;
using Fiap.Soat.VehicleManagement.Api.Shared.HealthChecks;
using Fiap.Soat.VehicleManagement.Api.Shared.Middlewares;
using Fiap.Soat.VehicleManagement.Application.Shared;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

_ = builder.Services.AddControllers();
_ = builder.Services.AddEndpointsApiExplorer();
_ = builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

// Add services to the container.
_ = builder.Services.AddServiceExtensions();
_ = builder.Services.AddRepositoryExtensions();
// _ = builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));
// _ = builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateProcessingFileStatusCommand).Assembly));
_ = builder.Services.AddHttpContextAccessor();
_ = builder.Services.AddHealthChecks()
    .AddCheck<DetailedHealthCheck>("detailed");
    // .AddDbContextCheck<AppDbContext>("database");
_ = builder.Services.AddRouting(options => options.LowercaseUrls = true);
_ = builder.Services.AddMemoryCache();
_ = builder.Services.AddInterfaceAdapters();

_ = builder.Services.AddOpenApi();
_ = builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
// _ = builder.Services.AddHttpClient();
_ = builder.Services.AddHttpLogging();

var app = builder.Build();

_ = app.UseCors("AllowAll");
_ = app.UseHttpLogging();

// Configure the HTTP request pipeline.
if ( app.Environment.IsDevelopment() )
{
    app.MapOpenApi();
}

_ = app.UseHttpsRedirection();

_ = app.MapScalarApiReference();
_ = app.UseMiddleware<ExceptionMiddleware>();
_ = app.UseHttpsRedirection();
_ = app.UseAuthorization();
_ = app.MapControllers();
_ = app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

_ = VehicleEndpoints.Map(app);

await app.RunAsync();

public partial class Program
{
    protected Program() { }
}
