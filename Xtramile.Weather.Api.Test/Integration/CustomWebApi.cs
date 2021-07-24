using System;
using System.Linq;
using FakeItEasy;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xtramile.Weather.Api.DataContext;
using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Test.Integration
{
    public class CustomWebApi<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var httpService = A.Fake<IHttpService>();
            builder.ConfigureServices(services =>
            {
                var countryContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(DbContextOptions<CountryContext>));
                services.Remove(countryContextDescriptor);
                var contextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(CountryContext));
                services.Remove(contextDescriptor);
                
                var httpServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                         typeof(IHttpService));
                services.Remove(httpServiceDescriptor);
                
                services.AddDbContext<CountryContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                });
                services.AddMediatR(typeof(Startup));
                services.AddSingleton(httpService);
                var serviceProvider = services.BuildServiceProvider();

                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var countryContext = scopedServices.GetRequiredService<CountryContext>();
                countryContext.Database.EnsureCreated();
            });
        }
    }
}