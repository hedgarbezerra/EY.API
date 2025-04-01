namespace EY.IntegrationTests;

public static class IPAddressGenerator
{
    private static readonly Random random = new();

    public static string Generate()
    {
        return string.Join(".", random.Next(0, 256), random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
    }
}