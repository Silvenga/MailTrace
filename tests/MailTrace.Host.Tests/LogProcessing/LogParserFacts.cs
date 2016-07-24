namespace MailTrace.Host.Tests.LogProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using MailTrace.Host.LogProcessing;
    using MailTrace.Host.Models.Logs;

    using Xunit;

    public class LogParserFacts
    {
        public static IEnumerable<object[]> LogLines => new List<object[]>
        {
            new object[]
            {
                "Jul 24 03:11:35 d0 postfix/smtp[2480]: 3A24217FBE9: to=<m@silvenga.com>, relay=silvenga-com.mail.protection.outlook.com[216.32.180.106]:25, delay=2.7, delays=0.75/0/0.16/1.8, dsn=2.6.0, status=sent (250 2.6.0 <20160724081126.59835.49549.718DED34@appveyor.com> [InternalId=725849473299, Hostname=CY4PR10MB1480.namprd10.prod.outlook.com] 9832 bytes in 0.332, 28.888 KB/sec Queued mail for delivery)",
                new DateTime(DateTime.Today.Year, 7, 24, 3, 11, 35),
                "d0",
                "postfix/smtp",
                "2480",
                "3A24217FBE9",
                "to=<m@silvenga.com>, relay=silvenga-com.mail.protection.outlook.com[216.32.180.106]:25, delay=2.7, delays=0.75/0/0.16/1.8, dsn=2.6.0, status=sent (250 2.6.0 <20160724081126.59835.49549.718DED34@appveyor.com> [InternalId=725849473299, Hostname=CY4PR10MB1480.namprd10.prod.outlook.com] 9832 bytes in 0.332, 28.888 KB/sec Queued mail for delivery)",
                new[]
                {
                    new LineAttribute("status", "sent (250 2.6.0 <20160724081126.59835.49549.718DED34@appveyor.com> [InternalId=725849473299, Hostname=CY4PR10MB1480.namprd10.prod.outlook.com] 9832 bytes in 0.332, 28.888 KB/sec Queued mail for delivery)")
                }
            }, new object[]
            {
                "Jul 24 03:11:33 d0 postfix/qmgr[13243]: 3A24217FBE9: from=<bounce+c4a080.b34ad-m=silvenga.com@appveyor.com>, size=2937, nrcpt=1 (queue active)",
                new DateTime(DateTime.Today.Year, 7, 24, 3, 11, 33),
                "d0",
                "postfix/qmgr",
                "13243",
                "3A24217FBE9",
                "from=<bounce+c4a080.b34ad-m=silvenga.com@appveyor.com>, size=2937, nrcpt=1 (queue active)",
                new[]
                {
                    new LineAttribute("from", "<bounce+c4a080.b34ad-m=silvenga.com@appveyor.com>")
                }
            },
            new object[]
            {
                "Jul 22 23:37:33 d0 postfix/smtp[22416]: 6A57017F4EF: to=<test@mxmirror.net>, relay=mxmirror.net[195.154.231.137]:25, delay=2.3, delays=0.86/0.01/1.1/0.35, dsn=2.0.0, status=sent (250 2.0.0 Ok: queued as 2F9C05EFE)",
                new DateTime(DateTime.Today.Year, 7, 22, 23, 37, 33),
                "d0",
                "postfix/smtp",
                "22416",
                "6A57017F4EF",
                "to=<test@mxmirror.net>, relay=mxmirror.net[195.154.231.137]:25, delay=2.3, delays=0.86/0.01/1.1/0.35, dsn=2.0.0, status=sent (250 2.0.0 Ok: queued as 2F9C05EFE)",
                new[]
                {
                    new LineAttribute("to", "<test@mxmirror.net>"),
                    new LineAttribute("relay", "mxmirror.net[195.154.231.137]:25"),
                    new LineAttribute("delay", "2.3"),
                    new LineAttribute("delays", "0.86/0.01/1.1/0.35"),
                    new LineAttribute("dsn", "2.0.0"),
                    new LineAttribute("status", "sent (250 2.0.0 Ok: queued as 2F9C05EFE)"),
                }
            },
            new object[]
            {
                "Jul 10 20:31:31 d0 postfix/smtpd[16283]: EF59F17F531: client=localhost[127.0.0.1]",
                new DateTime(DateTime.Today.Year, 7, 10, 20, 31, 31),
                "d0",
                "postfix/smtpd",
                "16283",
                "EF59F17F531",
                "client=localhost[127.0.0.1]",
                new[]
                {
                    new LineAttribute("client", "localhost[127.0.0.1]"),
                }
            },
            new object[]
            {
                "Jul 10 20:31:32 d0 postfix/cleanup[16286]: EF59F17F531: message-id=<sig.200092563b.SN1PR10MB06400081E699D2524DDED785D53F0@SN1PR10MB0640.namprd10.prod.outlook.com>",
                new DateTime(DateTime.Today.Year, 7, 10, 20, 31, 32),
                "d0",
                "postfix/cleanup",
                "16286",
                "EF59F17F531",
                "message-id=<sig.200092563b.SN1PR10MB06400081E699D2524DDED785D53F0@SN1PR10MB0640.namprd10.prod.outlook.com>",
                new[]
                {
                    new LineAttribute("message-id", "<sig.200092563b.SN1PR10MB06400081E699D2524DDED785D53F0@SN1PR10MB0640.namprd10.prod.outlook.com>")
                }
            },
            new object[]
            {
                "Jul 10 20:31:32 d0 opendkim[1708]: EF59F17F531: DKIM-Signature field added (s=2048_3_2015, d=silvenga.com)",
                new DateTime(DateTime.Today.Year, 7, 10, 20, 31, 32),
                "d0",
                "opendkim",
                "1708",
                "EF59F17F531",
                "DKIM-Signature field added (s=2048_3_2015, d=silvenga.com)",
                new LineAttribute[]
                {
                }
            },
            new object[]
            {
                "Jul 10 20:31:32 d0 postfix/smtpd[13724]: connect from localhost[127.0.0.1]",
                new DateTime(DateTime.Today.Year, 7, 10, 20, 31, 32),
                "d0",
                "postfix/smtpd",
                "13724",
                null,
                "connect from localhost[127.0.0.1]",
                new LineAttribute[]
                {
                }
            },
            new object[]
            {
                "Jul 20 00:40:16 d0 postfix/smtp[15103]: 6ECC317F531: to=<check-auth@verifier.port25.com>, relay=verifier.port25.com[38.95.177.200]:25, delay=0.87, delays=0.56/0.01/0.16/0.14, dsn=2.6.0, status=sent (250 2.6.0 message received)",
                new DateTime(DateTime.Today.Year, 7, 20, 0, 40, 16),
                "d0",
                "postfix/smtp",
                "15103",
                "6ECC317F531",
                "to=<check-auth@verifier.port25.com>, relay=verifier.port25.com[38.95.177.200]:25, delay=0.87, delays=0.56/0.01/0.16/0.14, dsn=2.6.0, status=sent (250 2.6.0 message received)",
                new[]
                {
                    new LineAttribute("to", "<check-auth@verifier.port25.com>"),
                    new LineAttribute("relay", "verifier.port25.com[38.95.177.200]:25"),
                    new LineAttribute("delay", "0.87"),
                    new LineAttribute("delays", "0.56/0.01/0.16/0.14"),
                    new LineAttribute("dsn", "2.6.0"),
                    new LineAttribute("status", "sent (250 2.6.0 message received)"),
                }
            },
        };

        [Theory, MemberData(nameof(LogLines))]
        public void Can_parse_connect_line(string line, DateTime time, string host, string serviceName, string servicePid, string queueId, string message,
                                           LineAttribute[] attributes)
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
            if (attributes.Any())
            {
                result.Attributes.Should().Contain(attributes);
            }
        }
    }
}