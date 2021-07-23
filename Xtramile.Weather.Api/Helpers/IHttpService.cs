using System;
using System.Threading.Tasks;

namespace Xtramile.Weather.Api.Helpers
{
    public interface IHttpService
    {
        Task<HttpServiceResult<T>> Get<T>(Uri uri, string token = null) where T : class;

        Task<HttpServiceResult<TResponse>> Post<TRequest, TResponse>(Uri uri, TRequest resource) where TRequest : class where TResponse : class;

        Task<HttpServiceResult<TResponse>> Put<TRequest, TResponse>(Uri uri, TRequest resource) where TRequest : class;

        Task<HttpServiceResult<TResponse>> Delete<TResponse>(Uri uri) where TResponse : class;
    }
}