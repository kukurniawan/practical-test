using Xtramile.Weather.Api.Helpers;

namespace Xtramile.Weather.Api.Features
{
    public class BaseResponse
    {
        internal bool IsSuccessful { get; set; } = true;
        internal int StatusCode { get; set; } = 200;
        public ErrorCode Error { get; set; }
    }
}