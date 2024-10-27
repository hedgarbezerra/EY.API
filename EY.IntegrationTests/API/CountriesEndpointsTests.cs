using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.IntegrationTests.API
{
    [TestFixture]
    public class CountriesEndpointsTests
    {
        private IntegrationsWebAppFactory _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _factory = new IntegrationsWebAppFactory();
            await _factory.StartContainersAsync();

            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _factory.DisposeAsync();
        }
    }
}
