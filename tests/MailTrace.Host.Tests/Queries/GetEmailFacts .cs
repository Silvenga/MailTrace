namespace MailTrace.Host.Tests.Queries
{
    using System.IO;

    using Effort.DataLoaders;

    using FluentAssertions;

    using MailTrace.Data;
    using MailTrace.Host.Queries.Emails;

    using Ploeh.AutoFixture;

    using Xunit;

    public class GetEmailFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly TraceContext _context;

        public GetEmailFacts()
        {
            var loader = new CsvDataLoader(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Defered"));

            var connection = Effort.DbConnectionFactory.CreateTransient(loader);
            _context = new TraceContext(connection);
        }

        [Fact]
        public void When_messageId_exists_return_email_and_attempts()
        {
            var query = new GetEmail.Query
            {
                MessageId = "<a1468908015.96715@assp.silvenga.lan>"
            };
            var handler = new GetEmailHandler(_context);

            // Act

            var result = handler.Handle(query);

            // Assert
            result.Should().NotBeNull();
            result.Client.Should().Be("localhost[127.0.0.1]");
            result.From.Should().Be("<dmarc-reporter@silvenga.com>");
            result.MessageId.Should().Be("<a1468908015.96715@assp.silvenga.lan>");
            result.Size.Should().Be("2018");
            result.To.Should().Be("<abuse@froninsa.win>");

            result.Attempts.Should().NotBeNull();

            result.Attempts.Should().Contain(x => x.Relay == "ducking.froninsa.win[72.9.103.238]:25");
            result.Attempts.Should().Contain(x => x.DsnCode == "4.4.2");
        }

        [Fact]
        public void When_message_does_not_exist_return_null()
        {
            var query = AutoFixture.Create<GetEmail.Query>();
            var handler = new GetEmailHandler(_context);

            // Act
            var result = handler.Handle(query);

            // Assert
            result.Should().BeNull();
        }
    }
}