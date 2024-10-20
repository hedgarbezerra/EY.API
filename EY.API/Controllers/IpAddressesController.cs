using EY.Domain.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EY.API.Controllers
{
    [Route("api/ipaddresses")]
    [ApiController]
    [EnableRateLimiting(RateLimitOptions.DEFAULT_POLICY)]
    public class IpAddressesController : ControllerBase
    {
    }
}
