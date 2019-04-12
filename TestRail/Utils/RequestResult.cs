using Newtonsoft.Json;
using System;
using System.Net;

namespace TestRail.Utils
{
    /// <summary>
    /// This class is used to simplify a HttpWebResponse and
    /// automatically deserialize the JSON returned.
    /// </summary>
    /// <typeparam name="T">The class to deserialized the JSON that is returned.</typeparam>
    public class RequestResult<T>
    {
        /// <summary>The type response code returned.</summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>The deserialized object from the JSON returned.</summary>
        public T Payload { get; }

        /// <summary>The exception that was throw when the response was received.</summary>
        public Exception ThrownException { get; }

        /// <summary>The raw JSON returned. </summary>
        public string RawJson { get; }

        /// <summary>Constructor for creating a new container for a web response.</summary>
        /// <param name="status">The type response code returned.</param>
        /// <param name="rawJson">The raw JSON to deserialize.</param>
        /// <param name="thrownException">The exception that was throw when the response was received.</param>
        public RequestResult(HttpStatusCode status, string rawJson = null, Exception thrownException = null)
        {
            if (rawJson != null)
            {
                RawJson = rawJson;
                Payload = JsonConvert.DeserializeObject<T>(RawJson);
            }

            StatusCode = status;
            ThrownException = thrownException;
        }
    }
}