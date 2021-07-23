using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Features.Weather
{
    public class Response : BaseResponse
    {
        public Coordinate Location { get; set; }
        public long LastUpdate { get; set; }
        public Wind Wind { get; set; }
        public int Visibility { get; set; }
        public double TemperatureCelsius { get; set; }
        public double TemperatureFahrenheit { get; set; }
        public double DewPoint { get; set; }
        public int Humidity { get; set; }
        public int Pressure { get; set; }
    }
}