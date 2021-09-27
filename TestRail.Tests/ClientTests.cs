using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TestRail.Types;
using Xunit;

namespace TestRail.Tests
{
    public class ClientTests
    {
        private readonly TestRailClient _client;

        public ClientTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            var section = configuration.GetSection("TestRail");

            _client = new TestRailClient(section["baseurl"], section["username"], section["password"]);
        }

        [Fact]
        public void Authentication_Is_Successful()
        {
            var response = _client.GetCase(1);

            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public void BulkResultsSerialization_Is_Successful()
        {
            var bulkResult = new BulkResults()
            {
                Results = new List<Result>()
                {
                    new Result()
                    {
                        CaseId = 1,
                        StatusId = 1UL,
                        Elapsed = new System.TimeSpan(0, 0, 10)
                    }
                }
            };

            var expectedJson = @"{
  ""results"": [
    {
      ""case_id"": 1,
      ""status_id"": 1,
      ""elapsed"": ""0d 0h 0m 10s""
    }
  ]
}";
            var json = bulkResult.GetJson().ToString();

            Assert.Equal(expectedJson, json);
        }
    }
}
