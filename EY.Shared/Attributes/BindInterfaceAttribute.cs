using Microsoft.Extensions.DependencyInjection;

namespace EY.Shared.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BindInterfaceAttribute : Attribute
{
    public BindInterfaceAttribute(Type @interface, ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        Interface = @interface;
        Lifetime = lifetime;
    }

    public Type Interface { get; set; }
    public ServiceLifetime Lifetime { get; set; }
}