using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Shared.Attributes
{
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
}
