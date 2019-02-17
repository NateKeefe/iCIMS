using System;
using System.Net;

namespace CDK.Common
{
    public class RESTRequestException : Exception
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }
        public string ContentAsString { get; set; }

        private RESTRequestException()
        {

        }

        public RESTRequestException(string reasonPhrase, HttpStatusCode statusCode, string contentAsString)
                : base(reasonPhrase)
        {
            StatusCode = (int)statusCode;
            HttpStatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            ContentAsString = contentAsString;
        }

        public RESTRequestException(string reasonPhrase, HttpStatusCode statusCode, string contentAsString, Exception innerException)
        : base(reasonPhrase, innerException)
        {
            HttpStatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            ContentAsString = contentAsString;
        }
    }
}
