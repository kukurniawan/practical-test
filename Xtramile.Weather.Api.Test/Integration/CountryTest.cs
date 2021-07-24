using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Shouldly;
using Xtramile.Weather.Api.DataContext;
using Xtramile.Weather.Api.Helpers;
using Xunit;

namespace Xtramile.Weather.Api.Test.Integration
{
    public class CountryTest : IClassFixture<CustomWebApi<Startup>>
    {
        private readonly CustomWebApi<Startup> _factory;
        private readonly CountryContext _countryContext;

        public CountryTest(CustomWebApi<Startup> factory)
        {
            _factory = factory;
            var serviceScope = _factory.Services.CreateScope();
            _countryContext = serviceScope.ServiceProvider.GetService<CountryContext>();
        }

        [Fact(DisplayName = "Get country list should not be founded")]
        public async Task CheckCountry1()
        {
            await _countryContext.Database.EnsureDeletedAsync();
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/country");
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound); 
            
            response.Content.Headers.ContentType.ToString().ShouldBe("application/json; charset=utf-8");
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldNotBeEmpty();

            var result = JsonConvert.DeserializeObject<ErrorCode>(responseString);
            result.ShouldNotBeNull();
            result.Description.ShouldBe("Country data is empty");
            result.Code.ShouldBe("0404");
        }
        
        [Fact(DisplayName = "Get country list should be founded")]
        public async Task CheckCountry2()
        {
            await _countryContext.Database.EnsureDeletedAsync();
            await _countryContext.Countries.AddAsync(new Country {Code = "ID", Name = "Fake Id"});
            await _countryContext.SaveChangesAsync();
            await _countryContext.Database.EnsureCreatedAsync();
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/country");
            response.StatusCode.ShouldBe(HttpStatusCode.OK); 
            
            response.Content.Headers.ContentType.ToString().ShouldBe("application/json; charset=utf-8");
            var responseString = await response.Content.ReadAsStringAsync();
            responseString.ShouldNotBeEmpty();

            var result = JsonConvert.DeserializeObject<Api.Features.CountryList.Response>(responseString);
            result.ShouldNotBeNull();

            var item = result.Data.ToList()[0];
            item.ShouldNotBeNull();
            item.Code.ShouldBe("ID");
            item.Name.ShouldBe("Fake Id");
        }
        
    }
}