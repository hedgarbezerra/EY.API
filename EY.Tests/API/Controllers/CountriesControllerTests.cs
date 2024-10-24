using EY.API.Controllers;
using EY.Domain.Contracts;
using EY.Domain.Entities;
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
    public class CountriesControllerTests
    {
        private CountriesController _controller;
        private ICountriesService _countriesService;

        [SetUp]
        public void SetUp()
        {
            _countriesService = Substitute.For<ICountriesService>();
            _controller = new CountriesController(_countriesService);
        }

        [Test]
        public async Task GetByThreeLetterCode_WhenCountryExists_ShouldReturnOk()
        {
            // Arrange
            var countryCode = "BRA";
            var country = new Country { Name = "", TwoLetterCode = "BR", ThreeLetterCode = "BRA" };
            var result = Result<Country>.Success(country);
            _countriesService.Get(countryCode).Returns(result);

            // Act
            var actionResult = await _controller.GetByThreeLetterCode(countryCode);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            var okResult = actionResult as OkObjectResult;
            okResult.Value.Should().Be(result);
        }

        [Test]
        public async Task GetByThreeLetterCode_WhenCountryDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var countryCode = "USA";
            var result = Result<Country>.Failure(new List<string> { "Country not found" });
            _countriesService.Get(countryCode).Returns(result);

            // Act
            var actionResult = await _controller.GetByThreeLetterCode(countryCode);

            // Assert
            actionResult.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = actionResult as NotFoundObjectResult;
            notFoundResult.Value.Should().Be(result);
        }

        [Test]
        public void GetPaginated_WhenSuccessful_ShouldReturnOk()
        {
            // Arrange
            var pagination = new PaginationInput(1);
            var paginatedCountries = new PaginatedList<Country>(new List<Country>().AsQueryable(), 1);
            var result = Result<PaginatedList<Country>>.Success(paginatedCountries);
            _countriesService.Get(pagination).Returns(result);

            // Act
            var actionResult = _controller.GetPaginated(pagination);

            // Assert
            actionResult.Should().BeOfType<OkObjectResult>();
            var okResult = actionResult as OkObjectResult;
            okResult.Value.Should().Be(result);
        }

        [Test]
        public void GetPaginated_WhenFails_ShouldReturnBadRequest()
        {
            // Arrange
            var pagination = new PaginationInput(1, 10);
            var result = Result<PaginatedList<Country>>.Failure(new List<string> { "Pagination error" });
            _countriesService.Get(pagination).Returns(result);

            // Act
            var actionResult = _controller.GetPaginated(pagination);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(result);
        }

        [Test]
        public void Add_WhenSuccessful_ShouldReturnCreated()
        {
            // Arrange
            var country = new CountryInput("Brazil", "BR", "BRA");
            var result = Result.Success();
            _countriesService.Add(country).Returns(result);

            // Act
            var actionResult = _controller.Add(country);

            // Assert
            actionResult.Should().BeOfType<CreatedAtActionResult>();
            var createdAtResult = actionResult as CreatedAtActionResult;
            createdAtResult.RouteValues["ThreeLetterCode"].Should().Be(country.ThreeLetterCode);
        }

        [Test]
        public void Add_WhenFails_ShouldReturnBadRequest()
        {
            // Arrange
            var country = new CountryInput("Brazil", "BR", "BRA");
            var result = Result.Failure(new List<string> { "Error adding country" });
            _countriesService.Add(country).Returns(result);

            // Act
            var actionResult = _controller.Add(country);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(result);
        }

        [Test]
        public void Update_WhenSuccessful_ShouldReturnNoContent()
        {
            // Arrange
            var country = new CountryInput("Brazil", "BR", "BRA");
            var result = Result.Success();
            _countriesService.Update(country).Returns(result);

            // Act
            var actionResult = _controller.Update(country);

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public void Update_WhenFails_ShouldReturnBadRequest()
        {
            // Arrange
            var country = new CountryInput("Brazil", "BR", "BRA");
            var result = Result.Failure(new List<string> { "Error updating country" });
            _countriesService.Update(country).Returns(result);

            // Act
            var actionResult = _controller.Update(country);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(result);
        }

        [Test]
        public void Delete_WhenSuccessful_ShouldReturnNoContent()
        {
            // Arrange
            var countryCode = "USA";
            var result = Result.Success();
            _countriesService.Delete(countryCode).Returns(result);

            // Act
            var actionResult = _controller.Delete(countryCode);

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public void Delete_WhenFails_ShouldReturnBadRequest()
        {
            // Arrange
            var countryCode = "USA";
            var result = Result.Failure(new List<string> { "Error deleting country" });
            _countriesService.Delete(countryCode).Returns(result);

            // Act
            var actionResult = _controller.Delete(countryCode);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = actionResult as BadRequestObjectResult;
            badRequestResult.Value.Should().Be(result);
        }

    }
}
