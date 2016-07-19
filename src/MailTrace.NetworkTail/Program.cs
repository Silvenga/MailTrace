namespace MailTrace.NetworkTail
{
    using System;
    using System.Linq;

    using MailTrace.NetworkTail.Service;

    class Program
    {
        static void Main(string[] args)
        {
            var tailer = new ChangeTailer(args.First());
            var breakLineBuffer = new BreakLineBuffer(tailer);
            var chunker = new ChangeChunker(breakLineBuffer);
            var transport = new TraceNetworkChangeTransporter(chunker, args.Last());

            tailer.Start();

            Console.WriteLine("Ready");
            Console.ReadLine();
        }
    }
}