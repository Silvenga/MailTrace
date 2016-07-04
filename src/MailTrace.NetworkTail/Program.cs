namespace MailTrace.NetworkTail
{
    using System;

    using MailTrace.NetworkTail.Service;

    class Program
    {
        static void Main(string[] args)
        {
            var tailer = new Tailer("C:\\Users\\Mark\\Desktop\\test.txt");

            tailer.Change += (sender, s) => Console.Write(s);

            tailer.Start();

            Console.ReadLine();
        }
    }
}