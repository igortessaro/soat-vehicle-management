using Fiap.Soat.VehicleManagement.Application.Adapters.Presenters;
using Fiap.Soat.VehicleManagement.Application.Mappers;
using Fiap.Soat.VehicleManagement.Application.Models;
using Fiap.Soat.VehicleManagement.Domain.Shared;
using Fiap.Soat.VehicleManagement.Domain.ValueObjects;
using System.Net;

namespace Fiap.Soat.VehicleManagement.Application.Tests;

public class PresentationTests
{
    private static readonly string[] PageItems = ["one", "two"];
    private static readonly int[] ExpectedItemLengths = [3, 3];

    [Test]
    public void VehiclePresenter_MapsVehicleToDetail()
    {
        var vehicle = ApplicationTestData.CreateVehicle(VehicleStatus.Sold);

        var detail = VehiclePresenter.ToDto(vehicle);

        Assert.Multiple(() =>
        {
            Assert.That(detail.Brand, Is.EqualTo(vehicle.Brand));
            Assert.That(detail.Model, Is.EqualTo(vehicle.Model));
            Assert.That(detail.LicensePlate, Is.EqualTo("ABC-1234"));
            Assert.That(detail.Status, Is.EqualTo(VehicleStatus.Sold));
            Assert.That(detail.Price, Is.EqualTo(vehicle.Price));
        });
    }

    [Test]
    public void VehiclePresenter_MapsSuccessfulResponse()
    {
        var vehicle = ApplicationTestData.CreateVehicle();

        var response = VehiclePresenter.ToDto(ResponseFactory.Ok(vehicle, HttpStatusCode.Created));

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.True);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Data.Brand, Is.EqualTo("Toyota"));
        });
    }

    [Test]
    public void VehiclePresenter_MapsFailedResponse()
    {
        var response = VehiclePresenter.ToDto(
            ResponseFactory.Fail<Domain.Entities.Vehicle>("Vehicle not found", HttpStatusCode.NotFound));

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Reasons[0].Message, Is.EqualTo("Vehicle not found"));
        });
    }

    [Test]
    public void VehiclePresenter_MapsVehicleList()
    {
        var vehicles = new[] { ApplicationTestData.CreateVehicle(), ApplicationTestData.CreateVehicle(VehicleStatus.Reserved) };

        var result = VehiclePresenter.ToDto(vehicles);

        Assert.That(result.Select(x => x.Status), Is.EqualTo(new[] { VehicleStatus.Available, VehicleStatus.Reserved }));
    }

    [Test]
    public void VehiclePresenter_MapsFailedVehicleListResponse()
    {
        var response = VehiclePresenter.ToDto(
            ResponseFactory.Fail<IReadOnlyList<Domain.Entities.Vehicle>>("Invalid filter", HttpStatusCode.BadRequest));

        Assert.Multiple(() =>
        {
            Assert.That(response.IsSuccess, Is.False);
            Assert.That(response.Reasons[0].Message, Is.EqualTo("Invalid filter"));
        });
    }

    [Test]
    public void ResponseMapper_MapsSuccessfulResponseAndPreservesStatusCode()
    {
        var response = ResponseFactory.Ok("vehicle", HttpStatusCode.Created);

        var mapped = ResponseMapper.Map(response, value => value.Length);

        Assert.Multiple(() =>
        {
            Assert.That(mapped.IsSuccess, Is.True);
            Assert.That(mapped.Data, Is.EqualTo(7));
            Assert.That(mapped.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        });
    }

    [Test]
    public void ResponseMapper_MapsFailedResponse()
    {
        var response = ResponseFactory.Fail<string>("Invalid vehicle", HttpStatusCode.UnprocessableEntity);

        var mapped = ResponseMapper.Map(response, value => value.Length);

        Assert.Multiple(() =>
        {
            Assert.That(mapped.IsSuccess, Is.False);
            Assert.That(mapped.StatusCode, Is.EqualTo(HttpStatusCode.UnprocessableEntity));
            Assert.That(mapped.Reasons[0].Message, Is.EqualTo("Invalid vehicle"));
        });
    }

    [Test]
    public void ResponseMapper_MapsPaginatedResponse()
    {
        var page = new Paginate<string>(PageItems, 10, 2, 1, 5);

        var mapped = ResponseMapper.Map(ResponseFactory.Ok(page), value => value.Length);

        Assert.Multiple(() =>
        {
            Assert.That(mapped.IsSuccess, Is.True);
            Assert.That(mapped.Data.Items, Is.EqualTo(ExpectedItemLengths));
            Assert.That(mapped.Data.TotalCount, Is.EqualTo(10));
            Assert.That(mapped.Data.PageSize, Is.EqualTo(2));
            Assert.That(mapped.Data.CurrentPage, Is.EqualTo(1));
            Assert.That(mapped.Data.TotalPages, Is.EqualTo(5));
        });
    }

    [Test]
    public void ResponseMapper_MapsFailedPaginatedResponse()
    {
        var response = ResponseFactory.Fail<Paginate<string>>("Invalid page", HttpStatusCode.BadRequest);

        var mapped = ResponseMapper.Map(response, value => value.Length);

        Assert.Multiple(() =>
        {
            Assert.That(mapped.IsSuccess, Is.False);
            Assert.That(mapped.Reasons[0].Message, Is.EqualTo("Invalid page"));
        });
    }
}
