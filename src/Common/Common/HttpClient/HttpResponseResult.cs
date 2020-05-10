

namespace Common
{
    using System.Net;

    /// <summary>
    /// Defines the class for HttpResponseResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HttpResponseResult<T>
    {
        public bool IsSuccessStatusCode { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public T Object { get; set; }
    }
}
