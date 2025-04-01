using EY.Shared.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EY.Tests.Shared.Attributes;

public class BindInterfaceAttributeTests
{
    [Test]
    public void BindInterfaceAttribute_Constructor_ShouldSetProperties()
    {
        // Arrange
        var interfaceType = typeof(IAsyncDisposable);
        var lifetime = ServiceLifetime.Singleton;

        // Act
        var attribute = new BindInterfaceAttribute(interfaceType, lifetime);

        // Assert
        attribute.Interface.Should().Be(interfaceType);
        attribute.Lifetime.Should().Be(lifetime);
    }

    [Test]
    public void BindInterfaceAttribute_DefaultLifetime_ShouldBeScoped()
    {
        // Arrange
        var interfaceType = typeof(IAsyncDisposable);

        // Act
        var attribute = new BindInterfaceAttribute(interfaceType);

        // Assert
        attribute.Interface.Should().Be(interfaceType);
        attribute.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }
}