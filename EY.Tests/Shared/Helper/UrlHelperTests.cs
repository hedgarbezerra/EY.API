using EY.Shared.Helper;
using Microsoft.AspNetCore.Http;

namespace EY.Tests.Shared.Helper;

[TestFixture]
public class UrlHelperTests
{
    [SetUp]
    public void SetUp()
    {
        _urlHelper = new UrlHelper();
    }

    private UrlHelper _urlHelper;

    [Test]
    public void GetDisplayUrl_ValidRequest_ReturnsCorrectUrl()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com");
        context.Request.Path = "/api/resource";
        context.Request.QueryString = new QueryString("?param=value");

        // Act
        var actualUrl = _urlHelper.GetDisplayUrl(context.Request);

        // Expected URL
        var expectedUrl = "https://example.com/api/resource?param=value";

        // Assert
        expectedUrl.Should().Be(actualUrl);
    }
}