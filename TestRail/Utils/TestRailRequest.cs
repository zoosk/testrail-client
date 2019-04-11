using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace TestRail.Utils
{
    public class TestRailRequest
    {
        private readonly HttpWebRequest _request;

        public TestRailRequest(string url, string requestType)
        {
            _request = (HttpWebRequest)WebRequest.Create(url);
            _request.AllowAutoRedirect = true;
            _request.UserAgent = "TestRail Client for .NET";
            _request.Method = requestType;
        }

        public void AddHeaders(IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                _request.Headers[header.Key] = header.Value;
            }
        }

        public void Accepts(string accept)
        {
            _request.Accept = accept;
        }

        public void ContentType(string contentType)
        {
            _request.ContentType = contentType;
        }

        public void AddBody(string bodyString)
        {
            var byteArray = Encoding.UTF8.GetBytes(bodyString);

            _request.ContentLength = byteArray.Length;

            var requestDataStream = _request.GetRequestStream();

            requestDataStream.Write(byteArray, 0, byteArray.Length);
            requestDataStream.Close();
        }

        public CommandResult Execute()
        {
            var response = (HttpWebResponse)_request.GetResponse();

            using (var reader = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException(), Encoding.UTF8))
            {
                var responseFromServer = reader.ReadToEnd();

                return new CommandResult(response.StatusCode == HttpStatusCode.OK, responseFromServer);
            }
        }
    }
}