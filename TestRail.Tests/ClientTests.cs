using Microsoft.Extensions.Configuration;
using System.IO;
using Xunit;

namespace TestRail.Tests
{
    public class ClientTests
    {
        private readonly IConfigurationRoot _configuration;

        public ClientTests()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        [Fact]
        public void Authentication_Is_Successful()
        {
            var section = _configuration.GetSection("TestRail");
            var client = new TestRailClient(section["baseurl"], section["username"], section["password"]);

            Assert.NotNull(client.Projects);
        }
    }
}
