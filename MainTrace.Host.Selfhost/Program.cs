namespace MainTrace.Host.Selfhost
{
    using System;

    using MailTrace.Host;

    using Microsoft.Owin.Hosting;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string baseAddress = "http://localhost:9900/";

            // Start OWIN host 
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("Ready");
                Console.ReadLine();
            }
        }
    }
}