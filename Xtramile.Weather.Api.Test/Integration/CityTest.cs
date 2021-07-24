using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using Xtramile.Weather.Api.DataContext;
using Xunit;

namespace Xtramile.Weather.Api.Test.Integration
{
    public class CityTest : IClassFixture<CustomWebApi<Startup>>
    {
        private readonly CustomWebApi<Startup> _factory;
        private readonly CountryContext _countryContext;

        public CityTest(CustomWebApi<Startup> factory)
        {
            _factory = factory;
            var serviceScope = _factory.Services.CreateScope();
            _countryContext = serviceScope.ServiceProvider.GetService<CountryContext>();
        }
        
        [Fact(DisplayName="Get country list should be founded")]
        public async Task CheckCity()
        {
            await _countryContext.Database.EnsureDeletedAsync();
            await _countryContext.Countries.AddAsync(new Country {Code = "ID", Name = "Fake Id"});
            await _countryContext.Cities.AddAsync(new City {CountryCode = "ID", Name = "Fake City"});
            await _countryContext.SaveChangesAsync();
            await _countryContext.Database.EnsureCreatedAsync();
            
            var client = _factory.CreateClient();
            var httpResponseMessage = await client.GetAsync("/city/ID");
            httpResponseMessage.EnsureSuccessStatusCode();
            
            httpResponseMessage.Content.Headers.ContentType.ToString().ShouldBe("application/json; charset=utf-8");
            var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
            responseString.ShouldNotBeEmpty();

            var response = JsonConvert.DeserializeObject<Api.Features.CityList.Response>(responseString);
            response.ShouldNotBeNull();
            var item = response.Data.ToList()[0];
            item.ShouldNotBeNull();
            item.Name.ShouldBe("Fake City");
            item.CountryCode.ShouldBe("ID");
            item.Id.ShouldBeGreaterThan(0);
        }
        
        [Fact(DisplayName="Get country list should not be founded")]
        public async Task CheckCity2()
        {
            await _countryContext.Database.EnsureDeletedAsync();
            
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/city/ID");
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            
            response.Content.Headers.ContentType.ToString().ShouldBe("application/json; charset=utf-8");
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldNotBeEmpty();
        }
    }
}