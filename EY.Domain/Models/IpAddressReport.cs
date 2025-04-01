namespace EY.Domain.Models;

public record IpAddressReportItem(string CountryName, int IpAddressesCount, DateTime LastIpAddressUpdatedAt);