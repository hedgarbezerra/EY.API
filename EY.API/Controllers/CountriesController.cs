using Asp.Versioning;
using EY.Domain.Contracts;
using EY.Domain.Models.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EY.API.Controllers
{
    [ApiVersion("1.0", Deprecated = false)]
    [Route("api/v{version:apiVersion}/countries")]
    [ApiController]
    [EnableRateLimiting(RateLimitOptions.DEFAULT_POLICY)]
    public class CountriesController : ControllerBase
    {

        [HttpGet]
        [Route("{name}")]
        public ActionResult Get([FromRoute] string name)
        {
            return Ok(name);
        }
    }
}
