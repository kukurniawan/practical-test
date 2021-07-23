using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryController
    {
        private readonly IMediator _mediator;

        public CountryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var response = await _mediator.Send(new Features.CountryList.Request());
            return ActionResultMapper.ToActionResult(response);
        }
    }
}