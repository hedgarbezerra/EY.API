using System.ComponentModel.DataAnnotations;
using EY.Domain.Models;

namespace EY.Tests.Domain.Models;

public class PaginationInputTests
{
    [Test]
    public void PaginationInput_ValidParameters_ShouldBeCreated()
    {
        // Arrange
        var index = 1;
        var size = 10;
        var query = "test";

        // Act
        var paginationInput = new PaginationInput(index, size, query);

        // Assert
        paginationInput.Index.Should().Be(index);
        paginationInput.Size.Should().Be(size);
        paginationInput.Query.Should().Be(query);
    }

    [Test]
    public void PaginationInput_ValidInput_ShouldBeValid()
    {
        // Arrange
        var paginationInput = new PaginationInput(1, 10, "search");

        // Act
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(paginationInput);
        var isValid = Validator.TryValidateObject(paginationInput, validationContext, validationResults, true);

        // Assert
        isValid.Should().BeTrue();
        validationResults.Should().BeEmpty();
    }
}

public class PaginatedListTests
{
    [Test]
    public void PaginatedList_ValidParameters_ShouldInitializeCorrectly()
    {
        // Arrange
        var source = Enumerable.Range(1, 100).AsQueryable();
        var pageIndex = 2;
        var pageSize = 10;

        // Act
        var paginatedList = new PaginatedList<int>(source, pageIndex, pageSize);

        // Assert
        paginatedList.PageIndex.Should().Be(pageIndex);
        paginatedList.PageSize.Should().Be(pageSize);
        paginatedList.TotalCount.Should().Be(100);
        paginatedList.TotalPages.Should().Be(10);
        paginatedList.Data.Should().HaveCount(pageSize);
        paginatedList.Data.Should().Contain(new List<int> { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
    }

    [Test]
    public void PaginatedList_FirstPage_ShouldNotHavePreviousPage()
    {
        // Arrange
        var source = Enumerable.Range(1, 100).AsQueryable();
        var pageIndex = 1;

        // Act
        var paginatedList = new PaginatedList<int>(source, pageIndex);

        // Assert
        paginatedList.HasPreviousPage.Should().BeFalse();
    }

    [Test]
    public void PaginatedList_LastPage_ShouldNotHaveNextPage()
    {
        // Arrange
        var source = Enumerable.Range(1, 100).AsQueryable();
        var pageIndex = 10;

        // Act
        var paginatedList = new PaginatedList<int>(source, pageIndex);

        // Assert
        paginatedList.HasNextPage.Should().BeFalse();
    }

    [Test]
    public void PaginatedList_EmptySource_ShouldInitializeCorrectly()
    {
        // Arrange
        var source = Enumerable.Empty<int>().AsQueryable();
        var pageIndex = 1;
        var pageSize = 10;

        // Act
        var paginatedList = new PaginatedList<int>(source, pageIndex, pageSize);

        // Assert
        paginatedList.TotalCount.Should().Be(0);
        paginatedList.TotalPages.Should().Be(0);
        paginatedList.Data.Should().BeEmpty();
    }
}