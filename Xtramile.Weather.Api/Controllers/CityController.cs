using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityController
    {
        private readonly IMediator _mediator;

        public CityController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("{countryCode}")]
        public async Task<IActionResult> List([FromRoute] string countryCode)
        {
            var response = await _mediator.Send(new Features.CityList.Request
            {
                CountryCode = countryCode
            });
            return ActionResultMapper.ToActionResult(response);
        }
    }
}