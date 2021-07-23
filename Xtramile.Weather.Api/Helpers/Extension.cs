using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Xtramile.Weather.Api.DataContext;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Xtramile.Weather.Api.Helpers
{
    public static class Extension
    {
        public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<CountryContext>();
            context.Database.Migrate();
            return builder;
        }
        
        public static IApplicationBuilder UseFailureMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FailureMiddleware>();
        }
        
        public static IApplicationBuilder InitDatabase(this IApplicationBuilder builder)
        {
            using var serviceScope = builder.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<CountryContext>();
            InitCountry(context);
            InitCity(context);
            return builder;
        }

        private static void InitCity(CountryContext context)
        {
            const string file = @"Data/init-city.json";
            if (!File.Exists(file)) return;
            var json = File.ReadAllText(file);
            var cities = JsonConvert.DeserializeObject<List<City>>(json);
            if (cities == null) return;
            foreach (var city in from city in cities
                let g = context.Countries.FirstOrDefault(c => c.Code == city.CountryCode)
                where g != null
                let f = context.Cities.FirstOrDefault(x => x.CountryCode == city.CountryCode && x.Name == city.Name)
                where f == null
                select city)
            {
                context.Cities.Add(city);
            }

            context.SaveChanges();
        }
        
        private static void InitCountry(CountryContext context)
        {
            const string file = @"Data/init-country.json";
            if (!File.Exists(file)) return;
            var json = File.ReadAllText(file);
            var countries = JsonConvert.DeserializeObject<List<Country>>(json);
            if (countries == null) return;
            foreach (var country in from country in countries
                let g = context.Countries.FirstOrDefault(x => x.Code == country.Code)
                where g == null
                select country)
            {
                context.Add(country);
            }
            context.SaveChanges();
        }
    }
}