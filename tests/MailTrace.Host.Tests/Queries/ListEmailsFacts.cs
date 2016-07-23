namespace MailTrace.Host.Tests.Queries
{
    using System;
    using System.IO;
    using System.Linq;

    using Effort.DataLoaders;

    using FluentAssertions;

    using MailTrace.Data;
    using MailTrace.Host.Queries.Emails;

    using Ploeh.AutoFixture;

    using Xunit;

    public class ListEmailsFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private const string AssetsDirectory = "Assets";

        [Fact]
        public void Can_get_list_of_emails()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), AssetsDirectory, "Defered");
            var loader = new CsvDataLoader(dataPath);

            var connection = Effort.DbConnectionFactory.CreateTransient(loader);
            var context = new TraceContext(connection);

            var query = new ListEmails.Query();
            var handler = new ListEmailsHandler(context);

            // Act

            var result = handler.Handle(query);

            // Assert
            result.Logs.Should().HaveCount(1);

            var email = result.Logs.First();
            email.From.Should().Be("<dmarc-reporter@silvenga.com>");
            email.FirstSeen.Should().Be(new DateTime(2016, 7, 19, 1, 5, 21)); // 
            email.MessageId.Should().Be("<a1468908015.96715@assp.silvenga.lan>");
            email.Size.Should().Be("2018");
            email.NumberOfRecipients.Should().Be("1 (queue active)");
            email.To.Should().Be("<abuse@froninsa.win>");
        }

        [Fact]
        public void When_multiple_tos_are_created_concat()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), AssetsDirectory, "MultipleTos");
            var loader = new CsvDataLoader(dataPath);

            var connection = Effort.DbConnectionFactory.CreateTransient(loader);
            var context = new TraceContext(connection);

            var query = new ListEmails.Query();
            var handler = new ListEmailsHandler(context);

            // Act

            var result = handler.Handle(query);

            // Assert
            result.Logs.Should().HaveCount(1);

            var email = result.Logs.First();
            email.From.Should().Be("<m@silvenga.com>");
            email.FirstSeen.Should().Be(new DateTime(2016, 7, 23, 16, 01, 00)); // 
            email.MessageId.Should().Be("<sig.501269a179.MWHPR10MB1485930B3BAD1143C8684A43D50B0@MWHPR10MB1485.namprd10.prod.outlook.com>");
            email.Size.Should().Be("4160");
            email.NumberOfRecipients.Should().Be("2 (queue active)");
            email.To.Should().Be("<m@silvenga.com>;<test@mxmirror.net>");
        }

        [Fact]
        public void When_filtering_by_to_filter_multi_to_emails()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), AssetsDirectory, "MultipleTos");
            var loader = new CsvDataLoader(dataPath);

            var connection = Effort.DbConnectionFactory.CreateTransient(loader);
            var context = new TraceContext(connection);

            var query = new ListEmails.Query
            {
                To = "<test@mxmirror.net>"
            };
            var handler = new ListEmailsHandler(context);

            // Act

            var result = handler.Handle(query);

            // Assert
            result.Logs.Should().HaveCount(1);

            var email = result.Logs.First();
            email.From.Should().Be("<m@silvenga.com>");
            email.FirstSeen.Should().Be(new DateTime(2016, 7, 23, 16, 01, 00)); // 
            email.MessageId.Should().Be("<sig.501269a179.MWHPR10MB1485930B3BAD1143C8684A43D50B0@MWHPR10MB1485.namprd10.prod.outlook.com>");
            email.Size.Should().Be("4160");
            email.NumberOfRecipients.Should().Be("2 (queue active)");
            email.To.Should().Be("<m@silvenga.com>;<test@mxmirror.net>");
        }

        [Fact]
        public void When_filtering_by_to_filter_result()
        {
            var dataPath = Path.Combine(Directory.GetCurrentDirectory(), AssetsDirectory, "MultipleTos");
            var loader = new CsvDataLoader(dataPath);

            var connection = Effort.DbConnectionFactory.CreateTransient(loader);
            var context = new TraceContext(connection);

            var query = new ListEmails.Query
            {
                To = AutoFixture.Create<string>()
            };
            var handler = new ListEmailsHandler(context);

            // Act

            var result = handler.Handle(query);

            // Assert
            result.Logs.Should().HaveCount(0);
        }
    }
}