using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xtramile.Weather.Api.DataContext;
using Xtramile.Weather.Api.Helpers;
using Xunit;

namespace Xtramile.Weather.Api.Test.Integration
{
    public class HostTest : IClassFixture<CustomWebApi<Startup>>
    {
        private readonly CustomWebApi<Startup> _factory;

        public HostTest(CustomWebApi<Startup> factory)
        {
            _factory = factory;
        }
        
        [Fact(DisplayName = "Service should not be null")]
        public void CheckService()
        {
            var httpService = _factory.Services.GetService<IHttpService>();
            var configuration = _factory.Services.GetService<IConfiguration>();
            var weatherHandler = _factory.Services
                .GetService<IRequestHandler<Api.Features.Weather.Request, Api.Features.Weather.Response>>();
            
            httpService.ShouldNotBeNull();
            configuration.ShouldNotBeNull();
            weatherHandler.ShouldNotBeNull();
        }
        
        [Fact(DisplayName = "Database provider should be in memory")]
        public void CheckDatabase()
        {
            var countryContext = _factory.Services.CreateScope().ServiceProvider.GetService<CountryContext>();
            countryContext.ShouldNotBeNull();
            countryContext.Database.IsInMemory().ShouldBeTrue();
        }
        
        [Fact(DisplayName = "Configuration should not be empty but was null")]
        public void CheckConfiguration()
        {
            var configuration = _factory.Services.GetService<IConfiguration>();
            configuration.GetValue<string>("OpenWeather:ApiKey").ShouldNotBeEmpty();
        }
    }
}