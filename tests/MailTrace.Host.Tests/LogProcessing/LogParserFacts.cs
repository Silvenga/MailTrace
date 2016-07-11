namespace MailTrace.Host.Tests.LogProcessing
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using MailTrace.Host.LogProcessing;

    using Xunit;

    public class LogParserFacts
    {
        public static IEnumerable<object[]> LogLines => new List<object[]>
        {
            new object[]
            {
                "Jul 10 20:31:31 d0 postfix/smtpd[16283]: EF59F17F531: client=localhost[127.0.0.1]",
                new DateTime(DateTime.Today.Year, 7, 10, 20, 31, 31),
                "d0",
                "postfix/smtpd",
                "16283",
                "EF59F17F531",
                "client=localhost[127.0.0.1]"
            },
            new object[]
            {
                "Jul 10 20:31:32 d0 postfix/cleanup[16286]: EF59F17F531: message-id=<sig.200092563b.SN1PR10MB06400081E699D2524DDED785D53F0@SN1PR10MB0640.namprd10.prod.outlook.com>",
                new DateTime(DateTime.Today.Year, 7, 10, 20, 31, 32),
                "d0",
                "postfix/cleanup",
                "16286",
                "EF59F17F531",
                "message-id=<sig.200092563b.SN1PR10MB06400081E699D2524DDED785D53F0@SN1PR10MB0640.namprd10.prod.outlook.com>"
            },
            new object[]
            {
                "Jul 10 20:31:32 d0 opendkim[1708]: EF59F17F531: DKIM-Signature field added (s=2048_3_2015, d=silvenga.com)",
                new DateTime(DateTime.Today.Year, 7, 10, 20, 31, 32),
                "d0",
                "opendkim",
                "1708",
                "EF59F17F531",
                "DKIM-Signature field added (s=2048_3_2015, d=silvenga.com)"
            },
            new object[]
            {
                "Jul 10 20:31:32 d0 postfix/smtpd[13724]: connect from localhost[127.0.0.1]",
                new DateTime(DateTime.Today.Year, 7, 10, 20, 31, 32),
                "d0",
                "postfix/smtpd",
                "13724",
                null,
                "connect from localhost[127.0.0.1]"
            }
        };

        [Theory, MemberData(nameof(LogLines))]
        public void Can_parse_connect_line(string line, DateTime time, string host, string serviceName, string servicePid, string queueId, string message)
        {
            var parser = new LogParser();

            // Act
            var result = parser.Parse(line);

            // Assert
            result.SourceTime.Should().BeCloseTo(time);
            result.Host.Should().Be(host);
            result.Service.Name.Should().Be(serviceName);
            result.Service.Pid.Should().Be(servicePid);
            result.QueueId.Should().Be(queueId);
            result.Message.Should().Be(message);
        }
    }
}