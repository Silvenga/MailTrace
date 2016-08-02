// ReSharper disable ObjectCreationAsStatement

namespace MailTrace.NetworkTail.Tests.Service
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using FluentAssertions;

    using MailTrace.NetworkTail.Interfaces;
    using MailTrace.NetworkTail.Models;
    using MailTrace.NetworkTail.Service;

    using NSubstitute;

    using Ploeh.AutoFixture;

    using Xunit;

    public class NetworkChangeTransporterFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();
        private readonly EventWaitHandle _waiter = new EventWaitHandle(false, EventResetMode.AutoReset);

        [Fact]
        public void Transport_should_set_base_address()
        {
            var mockClient = Substitute.For<HttpClientWraper>();
            var mockChangable = Substitute.For<IChangable>();
            var server = $"http://{AutoFixture.Create<string>()}/";

            // Act
            new TraceNetworkChangeTransporter(mockChangable, server, mockClient);

            // Assert
            mockClient.BaseAddress.ToString().Should().Be(server);
        }

        [Fact]
        public void When_receiveing_logs_send_via_network()
        {
            var mockClient = Substitute.For<IHttpClient>();
            mockClient.PostAsJsonAsync(Arg.Any<string>(), Arg.Any<object>())
                      .Returns(Task.FromResult(new HttpResponseMessage()))
                      .AndDoes(info => { _waiter.Set(); });

            var mockChangable = Substitute.For<IChangable>();
            var server = $"http://{AutoFixture.Create<string>()}/";

            var one = AutoFixture.Create<string>();
            var expected = new NetworkLogLinesCommand
            {
                LogLines = new[]
                {
                    one
                }
            };

            new TraceNetworkChangeTransporter(mockChangable, server, mockClient);

            // Act
            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(one));

            _waiter.WaitOne(100).Should().BeTrue();

            // Assert
            mockClient.Received().
                       PostAsJsonAsync(Arg.Is("/api/logs/import/networktail"), Arg.Do<NetworkLogLinesCommand>(x => { x.LogLines.ShouldBeEquivalentTo(expected.LogLines); }));
        }
    }
}