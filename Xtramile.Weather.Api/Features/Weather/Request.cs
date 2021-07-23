using MediatR;

namespace Xtramile.Weather.Api.Features.Weather
{
    public class Request : IRequest<Response>
    {
        public string CityName { get; set; }
        public string CountryCode { get; set; }
    }
}