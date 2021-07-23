using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Xtramile.Weather.Api.Helpers
{
    public class FailureMiddleware
    {
        private readonly RequestDelegate _next;

        public FailureMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task Invoke(HttpContext context)
        {
            var currentBody = context.Response.Body;


            await using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;
            ErrorCode error = null;
            try
            {
                await _next(context);
            }
            catch (ApiException e)
            {
                context.Response.StatusCode = e.StatusCode;
                error = new ErrorCode
                {
                    Code = e.ErrorCode,
                    Description = e.Message,
                };
            }
            catch (Exception e)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                error = new ErrorCode
                {
                    Code = "500",
                    Description = e.Message,
                };
            }

            context.Response.Body = currentBody;
            memoryStream.Seek(0, SeekOrigin.Begin);

            var readToEnd = await new StreamReader(memoryStream).ReadToEndAsync();
            if ( string.Compare(context.Response.ContentType, "image/png", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                var buffer = memoryStream.ToArray();
                await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
                return;
            }
            if (context.Response.StatusCode == 200 && context.Response.ContentType != null && !context.Response.ContentType.Contains("application/json"))
            {
                await context.Response.WriteAsync(readToEnd);
                return;
            }

            if (error != null)
            {
                context.Response.ContentType = "application/json;charset=utf-8";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(error));
            }
            else
            {
                await context.Response.WriteAsync(readToEnd);
            }
        }
    }
}