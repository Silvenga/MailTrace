namespace MailTrace.Host.Tests.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using FluentAssertions;

    using Microsoft.Owin.Testing;

    using Xunit;
    using Xunit.Abstractions;

    public class LogsControllerFacts : IDisposable
    {
        private readonly ITestOutputHelper _helper;
        private readonly TestServer _server;

        public LogsControllerFacts(ITestOutputHelper helper)
        {
            _helper = helper;
            _server = TestServer.Create<Startup>();
        }

        [Fact(Skip = "Not working")]
        public async Task Can_request_logs()
        {
            // Act
            var response = await _server.HttpClient.GetAsync("/api/logs/list?Before=2016/8/9");
            var body = await response.Content.ReadAsStringAsync();

            //var message = JObject.Parse(body);
            //var a = message.ToObject<HttpError>();
            //var error = message["InnerException"]["ExceptionMessage"].ToObject<string>();

            _helper.WriteLine(body);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}