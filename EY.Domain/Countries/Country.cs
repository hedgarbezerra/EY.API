using System.Text.Json.Serialization;
using EY.Domain.IpAddresses;

namespace EY.Domain.Countries;

//TODO: refatorar para termos um maior n�vel de abstra��o e tamb�m ter um dom�nio mais bem definido que n�o depende de primitivos.
public class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string TwoLetterCode { get; set; }
    public required string ThreeLetterCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore] public virtual ICollection<IpAddress> IpAddresses { get; set; } = new List<IpAddress>();
}