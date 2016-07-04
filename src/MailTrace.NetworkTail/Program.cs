namespace MailTrace.NetworkTail
{
    using System;
    using System.Linq;

    using MailTrace.NetworkTail.Service;

    class Program
    {
        static void Main(string[] args)
        {
            var tailer = new Tailer(args.First());

            tailer.Change += (sender, s) => Console.Write(s);

            tailer.Start();

            Console.ReadLine();
        }
    }
}