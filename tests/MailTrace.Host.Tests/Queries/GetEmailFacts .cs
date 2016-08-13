namespace MailTrace.Host.Tests.Queries
{
    using System.IO;

    using Effort.DataLoaders;

    using FluentAssertions;

    using MailTrace.Components.Queries.Emails;
    using MailTrace.Data;

    using Ploeh.AutoFixture;

    using Xunit;

    public class GetEmailFacts
    {
        private const string AssetsDirectory = "Assets";

        private static readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void When_messageId_exists_return_email_and_attempts()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), AssetsDirectory, "Defered");
            var loader = new CsvDataLoader(dataPath);

            var connection = Effort.DbConnectionFactory.CreateTransient(loader);
            var context = new TraceContext(connection);

            var query = new GetEmail.Query
            {
                MessageId = "<a1468908015.96715@assp.silvenga.lan>"
            };
            var handler = new GetEmailHandler(context);

            // Act

            var result = handler.Handle(query);

            // Assert
            result.Should().NotBeNull();
            result.Client.Should().Be("localhost[127.0.0.1]");
            result.From.Should().Be("<dmarc-reporter@silvenga.com>");
            result.MessageId.Should().Be("<a1468908015.96715@assp.silvenga.lan>");
            result.Size.Should().Be("2018");

            result.Attempts.Should().NotBeNull();

            result.Attempts.Should().Contain(x => x.Relay == "ducking.froninsa.win[72.9.103.238]:25");
            result.Attempts.Should().Contain(x => x.DsnCode == "4.4.2");
            result.Attempts.Should().Contain(x => x.To == "<abuse@froninsa.win>");
        }

        [Fact]
        public void When_multiple_recipient()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), AssetsDirectory, "MultipleTos");
            var loader = new CsvDataLoader(dataPath);

            var connection = Effort.DbConnectionFactory.CreateTransient(loader);
            var context = new TraceContext(connection);

            var query = new GetEmail.Query
            {
                MessageId = "<sig.501269a179.MWHPR10MB1485930B3BAD1143C8684A43D50B0@MWHPR10MB1485.namprd10.prod.outlook.com>"
            };
            var handler = new GetEmailHandler(context);

            // Act

            var result = handler.Handle(query);

            // Assert
            result.Should().NotBeNull();
            result.Attempts.Should().HaveCount(2);
        }

        [Fact]
        public void When_message_does_not_exist_return_null()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), AssetsDirectory, "Defered");
            var loader = new CsvDataLoader(dataPath);

            var connection = Effort.DbConnectionFactory.CreateTransient(loader);
            var context = new TraceContext(connection);

            var query = AutoFixture.Create<GetEmail.Query>();
            var handler = new GetEmailHandler(context);

            // Act
            var result = handler.Handle(query);

            // Assert
            result.Should().BeNull();
        }
    }
}