using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
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
        public void Get_Projects()
        {
            var response = _client.GetProjects();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Get_Cases()
        {
            var response = _client.GetCases(1, 1);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Get_Plans()
        {
            var response = _client.GetPlans(1);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public void Get_Suites()
        {
            var response = _client.GetSuites(1);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
