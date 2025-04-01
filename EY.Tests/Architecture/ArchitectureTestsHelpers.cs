using System.Reflection;
using System.Runtime.CompilerServices;

namespace EY.Tests.Architecture;

internal static class ArchitectureTestsHelpers
{
    public static string Name(this Assembly assembly)
    {
        return assembly.GetName()?.Name ?? string.Empty;
    }

    public static IEnumerable<string> Names(this IEnumerable<Assembly> assemblies)
    {
        return assemblies.Select(a => a.Name());
    }

    public static bool NameEndsWithInterfaceName(this Type type)
    {
        return type.GetInterfaces()
            .Any(interfaceType => type.Name.EndsWith(interfaceType.Name.TrimStart('I')));
    }

    public static (bool success, IEnumerable<Type> failedTypes) NamesEndsWithInterfaceName(this IEnumerable<Type> types)
    {
        return types.Where(t => t.NameEndsWithInterfaceName())
                .ToList() switch
            {
                [] => (true, []),
                [..] => (false, types)
            };
    }

    public static bool IsAnonymousType(this Type type)
    {
        return (Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute)) &&
                type.Name.Contains("AnonymousType")) ||
               (type.IsGenericType &&
                (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic);
    }
}