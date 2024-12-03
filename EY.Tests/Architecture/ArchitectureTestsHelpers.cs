using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Architecture
{
    internal static class ArchitectureTestsHelpers
    {

        public static string Name(this Assembly assembly) => assembly.GetName()?.Name ?? string.Empty;

        public static IEnumerable<string> Names(this IEnumerable<Assembly> assemblies) => assemblies.Select(a => a.Name());

        public static bool NameEndsWithInterfaceName(this Type type)
            => type.GetInterfaces()
                .Any(interfaceType => type.Name.EndsWith(interfaceType.Name.TrimStart('I')));

        public static (bool success, IEnumerable<Type> failedTypes) NamesEndsWithInterfaceName(this IEnumerable<Type> types)
            => types.Where(t => t.NameEndsWithInterfaceName())
                    .ToList() switch
            {
                [] => (true, []),
                [..] => (false, types)
            };

        public static bool IsAnonymousType(this Type type)
        {
            return Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)) &&
                   type.Name.Contains("AnonymousType") ||
                   type.IsGenericType &&
                   (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }
    }
}
