using MediatR;

namespace Xtramile.Weather.Api.Features.CityList
{
    public class Request : IRequest<Response>
    {
        public string CountryCode { get; set; }
    }
}