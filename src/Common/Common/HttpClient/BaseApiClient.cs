

namespace Common
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the class for BaseApiClient
    /// </summary>
    public abstract class BaseApiClient
    {
        protected BaseApiClient(ILogger<BaseApiClient> logger)
        {
            Logger = logger;
        }
        protected ILogger Logger;

        protected void LogResponseError(HttpResponseMessage response, string data)
        {
            Logger.LogError(
                LoggingEvents.Central,
                $"Request to [{response.RequestMessage.Method}] [{response.RequestMessage.RequestUri}] returns response with status code [{(int)response.StatusCode}] and data as {data}");
        }

        protected void LogResponse(HttpResponseMessage response, string data)
        {
            Logger.LogInformation(
                LoggingEvents.Central,
                $"Request to [{response.RequestMessage.Method}] [{response.RequestMessage.RequestUri}] returns response with status code [{(int)response.StatusCode}] and data as {data}");
        }

        protected async Task<HttpResponseResult<T>> CreateResponseResult<T>(HttpResponseMessage response, bool ignoreStatus = false)
        {
            var result = ignoreStatus ? await ReadResponseDataWithResult<T>(response) : await ReadResponseData<T>(response);
            return new HttpResponseResult<T>
            {
                IsSuccessStatusCode = response.IsSuccessStatusCode,
                StatusCode = response.StatusCode,
                Object = result
            };
        }

        private async Task<T> ReadResponseData<T>(HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                LogResponse(response, data);
                return JsonConvert.DeserializeObject<T>(data);
            }
            LogResponseError(response, data);
            return default(T);
        }

        private async Task<T> ReadResponseDataWithResult<T>(HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                LogResponse(response, data);
            }
            else
            {
                LogResponseError(response, data);
            }
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
}
