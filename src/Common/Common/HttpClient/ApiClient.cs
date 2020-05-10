
namespace Common
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class ApiClient : BaseApiClient
    {
        protected ApiClient(ILogger<BaseApiClient> logger)
            : base(logger)
        {
        }

        protected static HttpRequestMessage GenerateHttpRequestWithCorrelationId(HttpMethod method, string route, object content)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, CreateUri(route))
            {
                Headers = { { "X-Correlation-ID", CorrelationContextAccessor.CorrelationId } }
            };
            if (method != HttpMethod.Get)
            {
                httpRequestMessage.Content = CreateJsonContent(content);
            }
            return httpRequestMessage;
        }

        protected static async Task<HttpResponseMessage> PerformRequest(Func<Task<HttpResponseMessage>> requestAction)
        {
            return await requestAction.Invoke();
        }

        protected async Task<HttpResponseResult<TResult>> PerformRequestWithResult<TResult>(
            Func<Task<HttpResponseMessage>> requestAction, bool ignoreStatus = false)
        {
            var response = await PerformRequest(requestAction);
            return await CreateResponseResult<TResult>(response, ignoreStatus);
        }

        private static Uri CreateUri(string route)
        {
            return new Uri(route, UriKind.Relative);
        }

        private static StringContent CreateJsonContent(object content)
        {
            if (content != null)
            {
                return new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
            }
            return null;
        }
    }
}
