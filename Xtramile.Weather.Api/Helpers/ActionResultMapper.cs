using Microsoft.AspNetCore.Mvc;
using Xtramile.Weather.Api.Features;

namespace Xtramile.Weather.Api.Helpers
{
    public static class ActionResultMapper
    {
        private const string Message =
            "Something went wrong. Please try again in a few minutes or contact your support team.";
        public static IActionResult ToActionResult<TResponse>(TResponse result) where TResponse : BaseResponse
        {
            return result.IsSuccessful
                ? new ObjectResult(result)
                    { StatusCode = result.StatusCode }
                : result.Error == null
                    ? new ObjectResult(new ErrorCode { Description = Message })
                        { StatusCode = result.StatusCode }
                    : new ObjectResult(result.Error)
                        { StatusCode = result.StatusCode };
        }
    }
}