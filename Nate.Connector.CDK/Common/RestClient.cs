using System;
using System.IO;
using System.Net;
using System.Text;

public enum HttpVerb
{
    GET,
    POST,
    PUT,
    DELETE
}

namespace CDK.Common
{
    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public string AuthToken { get; set; }
        public string Accept { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string AuthType { get; set; }
        private CookieContainer cookieJar = new CookieContainer();

        public RestClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "application/json";
            PostData = "";
            AuthToken = "";
        }
        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "application/json";
            PostData = "";
            AuthToken = "";
        }
        public RestClient(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = "";
            AuthToken = "";
        }
        public RestClient(string endpoint, HttpVerb method, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postData;
            AuthToken = "";
        }
        public RestClient(string endpoint, HttpVerb method, string postData, string authToken)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "application/json";
            PostData = postData;
            AuthToken = authToken;
        }
        public string MakeRequest()
        {
            return MakeRequest("");
        }
        public string MakeRequest(string parameters)
        {
            return MakeRequest(EndPoint, parameters);
        }
        public string MakeRequest(string endpoint, string parameters)
        {
            EndPoint = endpoint;
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;
            request.CookieContainer = cookieJar;

            if (AuthType == "Basic")
            {
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(UserName + ":" + Password)));
            }
            if (AuthType == "HMAC")
            {
                //Build HMAC Signature with inputs
            }
            if (Accept == "")
            {
                Accept = "*/*";
            }
            request.Accept = Accept;

            if (!string.IsNullOrEmpty(PostData))
            {
                if (Method == HttpVerb.POST || Method == HttpVerb.PUT)
                {
                    var bytes = Encoding.GetEncoding("UTF-8").GetBytes(PostData);
                    request.ContentLength = bytes.Length;

                    using (var writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }

            //this grabs the response body
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                //return error
                if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }
                //201 = return Http Location Header
                if(response.StatusCode == HttpStatusCode.Created)
                {
                    responseValue = response.Headers.Get("Location");
                }
                //200 = return Http body
                if(response.StatusCode == HttpStatusCode.OK)
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                }
                return responseValue;
            }
        }
    }
}
