using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TestRail.Utils
{
    /// <summary>
    /// Wrapper class for building and sending an Http Web Request.
    /// </summary>
    public class TestRailRequest
    {
        private readonly HttpWebRequest _request;

        /// <summary>Constructor.</summary>
        /// <param name="url">URL for the request.</param>
        /// <param name="requestType">Designate the request being built as a GET or POST request.</param>
        public TestRailRequest(string url, string requestType)
        {
            _request = (HttpWebRequest)WebRequest.Create(url);
            _request.AllowAutoRedirect = true;
            _request.UserAgent = "TestRail Client for .NET";
            _request.Method = requestType;
        }

        /// <summary>Add headers to the request.</summary>
        /// <param name="headers">Key value pairs to be added to the headers.</param>
        public void AddHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                _request.Headers[header.Key] = header.Value;
            }
        }

        /// <summary>What type of data the request will accept.</summary>
        /// <param name="accept">Set the type, ex: 'application/json'.</param>
        public void Accepts(string accept)
        {
            _request.Accept = accept;
        }

        /// <summary>What type of data the request contains.</summary>
        /// <param name="contentType">Set the type, ex: 'application/json'.</param>
        public void ContentType(string contentType)
        {
            _request.ContentType = contentType;
        }

        /// <summary>Add a body to the request.</summary>
        /// <param name="bodyString">The text to add to the body.</param>
        public void AddBody(string bodyString)
        {
            var byteArray = Encoding.UTF8.GetBytes(bodyString);

            _request.ContentLength = byteArray.Length;

            var requestDataStream = _request.GetRequestStream();

            requestDataStream.Write(byteArray, 0, byteArray.Length);
            requestDataStream.Close();
        }

        /// <summary>Send the request and get a response.</summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <returns>If successful, will return a new RequestResult object.</returns>
        public RequestResult<T> Execute<T>()
        {
            var response = (HttpWebResponse)_request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8))
            {
                var responseFromServer = reader.ReadToEnd();

                return new RequestResult<T>(response.StatusCode, responseFromServer);
            }
        }
    }
}
