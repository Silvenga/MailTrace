namespace MailTrace.Host.LogProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using MailTrace.Host.Models.Logs;

    public class LogParser
    {
        private const string PostfixDateFormat = "MMM dd HH:mm:ss";

        public IEnumerable<LogLine> Parse(IEnumerable<string> lines)
        {
            return lines.Select(Parse);
        }

        public LogLine Parse(string line)
        {
            var result = new LogLine();

            var match = Regex.Match(line,
                @"^(?<date>.{15}) (?<host>\w+) (?<serviceName>.+)\[(?<servicePid>\d+)\]: ((?<queueId>.+): ){0,1}(?<message>.+)$");

            if (!match.Success)
            {
                throw new ArgumentException("Could not parse line.");
            }

            result.SourceTime = DateTime.ParseExact(match.Groups["date"].Value, PostfixDateFormat, CultureInfo.InvariantCulture);
            result.Host = match.Groups["host"].Value;
            result.Service = new PostfixService
            {
                Name = match.Groups["serviceName"].Value,
                Pid = match.Groups["servicePid"].Value
            };
            result.QueueId = match.Groups["queueId"].Success ? match.Groups["queueId"].Value : null;
            result.Message = match.Groups["message"].Value;

            result.Attributes = result.Service.Name.StartsWith("postfix")
                ? ParseTuples(result.Message).ToList()
                : new List<LineAttribute>();

            return result;
        }

        private IEnumerable<LineAttribute> ParseTuples(string message)
        {
            var tuples = message.Split(',').Select(x => x.Split('='));
            foreach (var tuple in tuples.Where(x => x.Length == 2))
            {
                yield return new LineAttribute(tuple.First(), tuple.Last());
            }
        }
    }
}