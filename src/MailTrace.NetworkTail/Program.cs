namespace MailTrace.NetworkTail
{
    using System;
    using System.Linq;

    using MailTrace.NetworkTail.Service;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            ConfigureLogging();

            var tailer = new ChangeTailer(args.First());
            var breakLineBuffer = new BreakLineBuffer(tailer);
            var chunker = new ChangeChunker(breakLineBuffer);
            var transport = new TraceNetworkChangeTransporter(chunker, args.Last());

            transport.Changed += (sender, eventArgs) => Logger.Info(eventArgs.Value);

            tailer.Start();

            Logger.Info("Ready");
            Console.ReadLine();
        }

        private static void ConfigureLogging()
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            consoleTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";
            fileTarget.FileName = "${basedir}/log";
            fileTarget.Layout = @"${date:format=HH\:mm\:ss} ${logger} ${message}";

            var rule1 = new LoggingRule("*", LogLevel.Debug, consoleTarget);
            config.LoggingRules.Add(rule1);

            var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule2);

            LogManager.Configuration = config;
        }
    }
}