using Asp.Versioning;
using EY.Domain.Contracts;
using EY.Domain.Entities;
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
        private readonly IUnitOfWork unitOfWork;

        public CountriesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }


        [HttpGet]
        [Route("{name}")]
        public ActionResult Get([FromRoute] string name)
        {
            var repo = unitOfWork.Repository<Country>();

            var country =  new Country() { Name = name , ThreeLetterCode = "NBA", TwoLetterCode = "NB"};
            return Ok(country);
        }
    }
}
