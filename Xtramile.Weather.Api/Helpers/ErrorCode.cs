using Newtonsoft.Json;

namespace Xtramile.Weather.Api.Helpers
{
    public class ErrorCode
    {
        [JsonProperty("message")]
        public string Description { get; set; }
        [JsonProperty("cod")]
        public string Code { get; set; }
    }
}