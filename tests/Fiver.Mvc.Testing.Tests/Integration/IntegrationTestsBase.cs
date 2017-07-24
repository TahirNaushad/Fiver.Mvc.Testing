using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Fiver.Mvc.Testing.Tests.Integration
{
    public class IntegrationTestsBase<TStartup> : IDisposable
        where TStartup : class
    {
        private readonly TestServer server;

        public IntegrationTestsBase()
        {
            var host = new WebHostBuilder()
                            .UseStartup<TStartup>()
                            .ConfigureServices(ConfigureServices);

            this.server = new TestServer(host);
            this.Client = this.server.CreateClient();
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            this.Client.Dispose();
            this.server.Dispose();
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        { }
    }
}
