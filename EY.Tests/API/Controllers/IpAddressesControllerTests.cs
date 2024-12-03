using EY.API.Controllers;
using EY.Domain.Contracts;
using EY.Domain.IpAddresses;
using EY.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.API.Controllers
{
    [TestFixture]
    public class IpAddressesControllerTests
    {
        private IpAddressesController _controller;
        private IIpAddressesService _ipAddressesService;

        [SetUp]
        public void SetUp()
        {
            _ipAddressesService = Substitute.For<IIpAddressesService>();
            _controller = new IpAddressesController(_ipAddressesService);
        }

        [Test]
        public async Task GetByIpAddress_WhenFound_ReturnsOk()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var result = Result<Ip2CResponse>.Success(new Ip2CResponse(ipAddress, "United States of America", "US", "USA"));

            _ipAddressesService.Get(ipAddress)
                .Returns(result);

            // Act
            var actionResult = await _controller.GetByIpAddress(ipAddress);

            // Assert
            var okResult = actionResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(result);
        }

        [Test]
        public async Task GetByIpAddress_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var result = Result<Ip2CResponse>.Failure("Not Found");

            _ipAddressesService.Get(ipAddress).Returns(Task.FromResult(result));

            // Act
            var actionResult = await _controller.GetByIpAddress(ipAddress);

            // Assert
            var notFoundResult = actionResult as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
            notFoundResult.Value.Should().Be(result);
        }

        [Test]
        public void GetPaginated_WhenSuccessful_ReturnsOk()
        {
            // Arrange
            var paginationInput = new PaginationInput(1, 10);
            var paginatedList = new PaginatedList<Ip2CResponse>(new List<Ip2CResponse>().AsQueryable(), 1, 10);
            var result = Result<PaginatedList<Ip2CResponse>>.Success(paginatedList);

            _ipAddressesService.Get(paginationInput).Returns(result);

            // Act
            var actionResult = _controller.GetPaginated(paginationInput);

            // Assert
            var okResult = actionResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(result);
        }

        [Test]
        public void GetPaginated_WhenFailed_ReturnsBadRequest()
        {
            // Arrange
            var paginationInput = new PaginationInput(1, 10);
            var result = Result<PaginatedList<Ip2CResponse>>.Failure("Invalid page");

            _ipAddressesService.Get(paginationInput).Returns(result);

            // Act
            var actionResult = _controller.GetPaginated(paginationInput);

            // Assert
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(result);
        }

        [Test]
        public void Add_WhenSuccessful_ReturnsCreatedAtAction()
        {
            // Arrange
            var ipAddress = new IpAddressInput("192.168.0.1", "Brazil", "BR", "BRA");
            var result = Result.Success();

            _ipAddressesService.Add(ipAddress).Returns(result);

            // Act
            var actionResult = _controller.Add(ipAddress);

            // Assert
            var createdAtActionResult = actionResult as CreatedAtActionResult;
            createdAtActionResult.Should().NotBeNull();
            createdAtActionResult!.StatusCode.Should().Be(201);
            createdAtActionResult.ActionName.Should().Be(nameof(IpAddressesController.GetByIpAddress));
            createdAtActionResult.RouteValues["IpAddress"].Should().Be(ipAddress.IpAddress);
        }

        [Test]
        public void Add_WhenFailed_ReturnsBadRequest()
        {
            // Arrange
            var ipAddress = new IpAddressInput("192.168.0.1", "Brazil", "BR", "BRA");
            var result = Result.Failure("Add failed");

            _ipAddressesService.Add(ipAddress).Returns(result);

            // Act
            var actionResult = _controller.Add(ipAddress);

            // Assert
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(result);
        }

        [Test]
        public void Update_WhenSuccessful_ReturnsNoContent()
        {
            // Arrange
            var ipAddress = new IpAddressInput("192.168.0.1", "Brazil", "BR", "BRA");
            var result = Result.Success();

            _ipAddressesService.Update(ipAddress).Returns(result);

            // Act
            var actionResult = _controller.Update(ipAddress);

            // Assert
            var noContentResult = actionResult as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult!.StatusCode.Should().Be(204);
        }

        [Test]
        public void Update_WhenFailed_ReturnsBadRequest()
        {
            // Arrange
            var ipAddress = new IpAddressInput("192.168.0.1", "Brazil", "BR", "BRA");
            var result = Result.Failure("Update failed");

            _ipAddressesService.Update(ipAddress).Returns(result);

            // Act
            var actionResult = _controller.Update(ipAddress);

            // Assert
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(result);
        }

        [Test]
        public void Delete_WhenSuccessful_ReturnsNoContent()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var result = Result.Success();

            _ipAddressesService.Delete(ipAddress).Returns(result);

            // Act
            var actionResult = _controller.Delete(ipAddress);

            // Assert
            var noContentResult = actionResult as NoContentResult;
            noContentResult.Should().NotBeNull();
            noContentResult!.StatusCode.Should().Be(204);
        }

        [Test]
        public void Delete_WhenFailed_ReturnsBadRequest()
        {
            // Arrange
            var ipAddress = "192.168.0.1";
            var result = Result.Failure("Delete failed");

            _ipAddressesService.Delete(ipAddress).Returns(result);

            // Act
            var actionResult = _controller.Delete(ipAddress);

            // Assert
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(result);
        }

        [Test]
        public void GetReport_WhenSuccessful_ReturnsOk()
        {
            // Arrange
            var countries = new[] { "US", "CA" };
            var reportResult = new List<IpAddressReportItem>();
            var result = Result<List<IpAddressReportItem>>.Success(reportResult);

            _ipAddressesService.Report(countries).Returns(result);

            // Act
            var actionResult = _controller.GetReport(countries);

            // Assert
            var okResult = actionResult as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().Be(result);
        }

        [Test]
        public void GetReport_WhenFailed_ReturnsBadRequest()
        {
            // Arrange
            var countries = new[] { "US", "CA" };
            var result = Result<List<IpAddressReportItem>>.Failure("Report generation failed");

            _ipAddressesService.Report(countries).Returns(result);

            // Act
            var actionResult = _controller.GetReport(countries);

            // Assert
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
            badRequestResult.Value.Should().Be(result);
        }
    }

}
