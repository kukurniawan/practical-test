using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Xtramile.Weather.Api.Features.Weather
{
    public class OpenWeatherResponse
    {
        [JsonProperty("coord")]
        public Coordinate Coordinate { get; set; }
        public IEnumerable<Weather> Weather { get; set; }
        public string Base { get; set; }
        [JsonProperty("main")]
        public MainWeather MainWeather { get; set; }
        public int Visibility { get; set; }
        public Wind Wind { get; set; }
        [JsonProperty("dt")]
        public long LastUpdate { get; set; }
        public Cloud Clouds { get; set; }
    }

    public class Coordinate
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }

    public class Weather
    {
        public int Id { get; set; }
        public string Main { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }

    public class MainWeather
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }
        public int Pressure { get; set; }
        public int Humidity { get; set; }
        [JsonProperty("temp_min")]
        public double TemperatureMinimum { get; set; }
        [JsonProperty("temp_max")]
        public double TemperatureMaximum { get; set; }
        public double TemperatureCelsius()
        {
            return Temperature - 273.15;
        }
        
        public double TemperatureFahrenheit()
        {
            return (Temperature-273.15) * 9/5 + 32;
        }
        
        public double DewPoint()
        {
            var tA = Math.Pow(Humidity/100, 1/8);
            return (112 + 0.9 * TemperatureCelsius()) * tA + 0.1 * TemperatureCelsius() - 112;
        }
    }

    public class Wind
    {
        public double Speed { get; set; }
        [JsonProperty("deg")]
        public int Degrees { get; set; }
    }

    public class Cloud
    {
        public int All { get; set; }
    }
    
}