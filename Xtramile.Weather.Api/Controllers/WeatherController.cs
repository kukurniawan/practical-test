using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("{cityName}")]
        public async Task<IActionResult> GetByCity([FromRoute] string cityName)
        {
            var response = await _mediator.Send(new Features.Weather.Request
            {
                CityName = cityName
            });
            return ActionResultMapper.ToActionResult(response);
        }
        
        [HttpGet("{countryCode}/{cityName}")]
        public async Task<IActionResult> GetByCountryCity([FromRoute]string countryCode,[FromRoute] string cityName)
        {
            var response = await _mediator.Send(new Features.Weather.Request
            {
                CityName = cityName,
                CountryCode = countryCode
            });
            return ActionResultMapper.ToActionResult(response);
        }
    }
}