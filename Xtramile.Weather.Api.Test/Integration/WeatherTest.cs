using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using Xtramile.Weather.Api.DataContext;
using Xtramile.Weather.Api.Features.Weather;
using Xtramile.Weather.Api.Helpers;
using Xunit;

namespace Xtramile.Weather.Api.Test.Integration
{
    public class WeatherTest : IClassFixture<CustomWebApi<Startup>>
    {
        private readonly CustomWebApi<Startup> _factory;
        private readonly CountryContext _countryContext;
        private readonly IHttpService _httpService;

        public WeatherTest(CustomWebApi<Startup> factory)
        {
            _factory = factory;
            _httpService = _factory.Services.GetService<IHttpService>();
            var serviceScope = _factory.Services.CreateScope();
            _countryContext = serviceScope.ServiceProvider.GetService<CountryContext>();
        }
        
        [Fact(DisplayName = "Get weather should be internal server error when there are no internet connection")]
        public async Task CheckWeather1()
        {
            A.CallTo(() =>
                    _httpService.Get<OpenWeatherResponse>(A<Uri>.Ignored, A<string>.Ignored))
                .Throws(new Exception());
            await _countryContext.Database.EnsureDeletedAsync();
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/weather/Fake");
            response.StatusCode.ShouldBe(HttpStatusCode.InternalServerError); 
            
            response.Content.Headers.ContentType.ToString().ShouldBe("application/json; charset=utf-8");
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldNotBeEmpty();
        }
        
        [Fact(DisplayName = "Get weather should be not founded when there are no city on OpenWeatherApi")]
        public async Task CheckWeather2()
        {
            A.CallTo(() =>
                    _httpService.Get<OpenWeatherResponse>(A<Uri>.Ignored, A<string>.Ignored))
                .Returns(HttpServiceResult<OpenWeatherResponse>.Fail("FAKE", "FAKE", 404));
            await _countryContext.Database.EnsureDeletedAsync();
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync("/weather/Fake");
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound); 
            
            response.Content.Headers.ContentType.ToString().ShouldBe("application/json; charset=utf-8");
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldNotBeEmpty();

            var result = JsonConvert.DeserializeObject<ErrorResponse>(responseString);
            result.ShouldNotBeNull();
            result.Description.ShouldBe("FAKE");
            result.Code.ShouldBe("FAKE");
        }
        
        [Fact(DisplayName = "Get weather should be founded ")]
        public async Task CheckWeather3()
        {
            A.CallTo(() =>
                    _httpService.Get<OpenWeatherResponse>(A<Uri>.Ignored, A<string>.Ignored))
                .Returns(HttpServiceResult<OpenWeatherResponse>.Ok(GetResponse(), 200));
            await _countryContext.Database.EnsureDeletedAsync();
            var client = _factory.CreateClient();
            
            var response = await client.GetAsync("/weather/Fake");
            response.StatusCode.ShouldBe(HttpStatusCode.OK); 
            
            response.Content.Headers.ContentType.ToString().ShouldBe("application/json; charset=utf-8");
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldNotBeEmpty();

            var result = JsonConvert.DeserializeObject<Response>(responseString);
            result.ShouldNotBeNull();
            result.Humidity.ShouldBe(0);
            result.Location.ShouldNotBeNull();
            result.Location.Latitude.ShouldBe(0);
            result.Location.Longitude.ShouldBe(0);
            result.Pressure.ShouldBe(0);
            result.Visibility.ShouldBe(0);
            result.Wind.ShouldNotBeNull();
            result.Wind.Degrees.ShouldBe(0);
            result.Wind.Speed.ShouldBe(0);
            result.DewPoint.ShouldBe(-274);
            result.LastUpdate.ShouldBe(0);
            result.TemperatureCelsius.ShouldBe(-274);
            result.TemperatureFahrenheit.ShouldBe(-460);
        }

        private static OpenWeatherResponse GetResponse()
        {
            return new OpenWeatherResponse
            {
                Base = "FAKE",
                Clouds = new Cloud
                {
                    All = 0
                },
                Coordinate = new Coordinate
                {
                    Latitude = 0,
                    Longitude = 0
                },
                Visibility = 0,
                Weather = new List<Api.Features.Weather.Weather>
                {
                    new Api.Features.Weather.Weather
                    {
                        Description = "FAKE", Icon = "FAKE", Id = 0, Main = "FAKE"
                    }
                },
                Wind = new Wind
                {
                    Degrees = 0, Speed = 0
                },
                LastUpdate = 0,
                MainWeather = new MainWeather
                {
                    Humidity = 0, Pressure = 0, Temperature = 0, TemperatureMaximum = 0, TemperatureMinimum = 0
                }
            };
        }
    }

    public class ErrorResponse
    {
        public string Description { get; set; }
        public string Code { get; set; }
    }
}