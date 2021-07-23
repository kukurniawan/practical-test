using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Xtramile.Weather.Api.Helpers
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _client;
        private readonly JsonSerializerSettings _serializerSettings;
        public HttpService()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task<HttpServiceResult<T>> Get<T>(Uri uri, string token = null) where T : class
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return HttpServiceResult<T>.Ok(JsonConvert.DeserializeObject<T>(result), (int)response.StatusCode);
            
            var failedJson = JsonConvert.DeserializeObject<ErrorCode>(result);

            return failedJson != null && response.StatusCode == HttpStatusCode.NotFound
                ? HttpServiceResult<T>.Fail(failedJson.Description, failedJson.Code, (int)response.StatusCode)
                : HttpServiceResult<T>.Fail($"Error occurred while performing post to {uri}: {response} - {result}", null, (int)response.StatusCode);
        }

        public async Task<HttpServiceResult<TResponse>> Post<TRequest, TResponse>(Uri uri, TRequest resource) where TRequest : class where TResponse : class
        {
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            var content = JsonConvert.SerializeObject(resource, _serializerSettings);
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                return HttpServiceResult<TResponse>.Ok(JsonConvert.DeserializeObject<TResponse>(result), (int)response.StatusCode);

            var failedJson = JsonConvert.DeserializeObject<ErrorCode>(result);

            return failedJson != null
                ? HttpServiceResult<TResponse>.Fail(failedJson.Description, failedJson.Code, (int)response.StatusCode, failedJson)
                : HttpServiceResult<TResponse>.Fail($"Error occurred while performing post to {uri}: {response} - {result}", null, (int)response.StatusCode);
        }

        public async Task<HttpServiceResult<TResponse>> Put<TRequest, TResponse>(Uri uri, TRequest resource) where TRequest : class
        {
            var request = new HttpRequestMessage(HttpMethod.Put, uri);
            var content = JsonConvert.SerializeObject(resource, _serializerSettings);
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.SendAsync(request);
            var resultSerialized = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return HttpServiceResult<TResponse>.Ok(JsonConvert.DeserializeObject<TResponse>(resultSerialized), (int)response.StatusCode);

            var failedJson = JsonConvert.DeserializeObject<ErrorCode>(resultSerialized);

            return failedJson != null
                ? HttpServiceResult<TResponse>.Fail(failedJson.Description, failedJson.Code, (int)response.StatusCode, failedJson)
                : HttpServiceResult<TResponse>.Fail($"Error occurred while performing put to {uri}: {response} - {resultSerialized}", null, (int)response.StatusCode);
        }
        
        public async Task<HttpServiceResult<TResponse>> Delete<TResponse>(Uri uri) where TResponse : class
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, uri);
            var response = await _client.SendAsync(request);
            var resultSerialized = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
                return HttpServiceResult<TResponse>.NoContent(JsonConvert.DeserializeObject<TResponse>(resultSerialized), (int)response.StatusCode);

            var failedJson = JsonConvert.DeserializeObject<ErrorCode>(resultSerialized);

            return failedJson != null
                ? HttpServiceResult<TResponse>.Fail(failedJson.Description, failedJson.Code, (int)response.StatusCode)
                : HttpServiceResult<TResponse>.Fail($"Error occurred while performing put to {uri}: {response} - {resultSerialized}", null, (int)response.StatusCode);
        }
    }
}