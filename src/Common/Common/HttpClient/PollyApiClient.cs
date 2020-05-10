

namespace Common
{
    using Microsoft.Extensions.Logging;
    using Polly;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the class for PollyApiClient
    /// </summary>
    public abstract class PollyApiClient : BaseApiClient
    {
        protected PollyApiClient(ILogger<BaseApiClient> logger)
            : base(logger)
        { }

        protected long ResilienceDelayInMilliSeconds { get; set; }

        protected int ResilienceRetriesCount { get; set; }

        protected static Dictionary<string, object> CreateContext(string data, string action)
        {
            return new Dictionary<string, object>
            {
                { "Data", data},
                {"Action", action }
            };
        }

        protected async Task<HttpResponseMessage> PerformRequest(Func<Task<HttpResponseMessage>> requestAction, Dictionary<string, object> context)
        {
            HttpStatusCode[] httpStatusCodesWorthRetrying =
            {
                HttpStatusCode.RequestTimeout,
                HttpStatusCode.InternalServerError,
                HttpStatusCode.BadGateway,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.GatewayTimeout
            };
            return await Policy
                .Handle<AggregateException>(HandleException)
                .Or<HttpRequestException>()
                .Or<WebException>()
                .OrResult<HttpResponseMessage>(message => httpStatusCodesWorthRetrying.Contains(message.StatusCode))
                .WaitAndRetryAsync(ResilienceRetriesCount, GetResilienceTimeout, OnRetry)
                .ExecuteAsync(requestAction);
        }

        protected async Task<HttpResponseResult<TResult>> PerformRequestWithResult<TResult>(
            Func<Task<HttpResponseMessage>> requestAction,
            Dictionary<string, object> context)
        {
            var response = await PerformRequest(requestAction, context);
            return await CreateResponseResult<TResult>(response);
        }

        private static bool HandleException(AggregateException aggregateException)
        {
            return aggregateException.InnerExceptions.Any(
                exception =>
                    exception is HttpRequestException ||
                    exception is WebException);
        }

        private TimeSpan GetResilienceTimeout(int retryNumber)
        {
            return TimeSpan.FromMilliseconds(ResilienceDelayInMilliSeconds * retryNumber);
        }

        private Task OnRetry(
            DelegateResult<HttpResponseMessage> result,
            TimeSpan timeSpan,
            int retriesCount,
            Context context)
        {
            if (result.Exception != null)
            {
                Logger.LogError(
                    LoggingEvents.Central,
                    result.Exception,
                    $"Error occurred on {context["Action"]}: {context["Data"]}. Error message: {result.Exception.Message}");
            }
            if (result.Result != null)
            {
                Logger.LogError(
                     LoggingEvents.Central,
                     result.Exception,
                     $"Invalid response status code {result.Result.StatusCode} on {context["Action"]}: {context["Data"]}.");
            }

            Logger.LogWarning(LoggingEvents.Central, $"Retry number {retriesCount} to {context["Action"]}: {context["Data"]}.");
            return Task.CompletedTask;
        }
    }
}
