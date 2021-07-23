using System;

namespace Xtramile.Weather.Api.Helpers
{
    public class ApiException : Exception
    {
        public ApiException(string message, string errorCode, int httpStatusCode) : base(message)
        {
            ErrorCode = errorCode;
            StatusCode = httpStatusCode;
        }

        public int StatusCode { get; set; }

        public string ErrorCode { get; set; }
    }
}