using System.Net;

namespace HeladeriaAPI.Utils
{
    public class HttpError : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public HttpError(string Message, HttpStatusCode httpStatusCode) : base(Message)
        {
            StatusCode = httpStatusCode;
        }
    }
}
    