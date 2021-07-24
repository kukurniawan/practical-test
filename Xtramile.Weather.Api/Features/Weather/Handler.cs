using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Features.Weather
{
    public class Handler : IRequestHandler<Request, Response>
    {
        private readonly IHttpService _httpService;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public Handler(IHttpService httpService, IConfiguration configuration)
        {
            _httpService = httpService;
            _baseUrl = configuration.GetValue<string>("OpenWeather:BaseUrl");
            _apiKey = configuration.GetValue<string>("OpenWeather:ApiKey");
        }
        
        public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
        {
            var country = string.IsNullOrEmpty(request.CountryCode) ? "" : $",{request.CountryCode}";
            var uri = new Uri($"{_baseUrl}/weather?q={request.CityName}{country}&APPID={_apiKey}");
            var result = await _httpService.Get<OpenWeatherResponse>(uri);
            if (!result.IsSuccessful) return new Response
            {
                Error = new ErrorCode
                {
                    Code = result.ErrorCode,
                    Description = result.ErrorDescription
                },
                StatusCode = result.HttpStatusCode,
                IsSuccessful = false
            };
            var response = new Response 
            {
                Humidity = result.Value.MainWeather.Humidity,
                Location = result.Value.Coordinate,
                Pressure = result.Value.MainWeather.Pressure,
                Visibility = result.Value.Visibility,
                Wind = result.Value.Wind,
                LastUpdate = result.Value.LastUpdate,
                TemperatureCelsius = Math.Floor(result.Value.MainWeather.TemperatureCelsius()),
                TemperatureFahrenheit = Math.Floor(result.Value.MainWeather.TemperatureFahrenheit()),
                DewPoint = Math.Ceiling(result.Value.MainWeather.DewPoint()),
                IsSuccessful = true,
                StatusCode = result.HttpStatusCode,
                Clouds = result.Value.Clouds
            };
            return response;
        }

    }
}