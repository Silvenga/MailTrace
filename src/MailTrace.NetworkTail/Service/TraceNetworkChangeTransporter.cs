namespace MailTrace.NetworkTail.Service
{
    using System;

    using MailTrace.NetworkTail.Interfaces;
    using MailTrace.NetworkTail.Models;

    public class TraceNetworkChangeTransporter : IChangable
    {
        private readonly IChangable _changable;
        private readonly IHttpClient _client;

        public event EventHandler<ChangedEventArgs> Changed;

        public TraceNetworkChangeTransporter(IChangable changable, string traceServer, IHttpClient client = null)
        {
            _changable = changable;
            _client = client ?? new HttpClientWraper();
            _client.BaseAddress = new Uri(traceServer);
            _changable.Changed += ChangableOnChanged;
        }

        private void ChangableOnChanged(object sender, ChangedEventArgs changedEventArgs)
        {
            OnChanged(changedEventArgs);

            try
            {
                var lines = changedEventArgs.Value.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

                var command = new NetworkLogLinesCommand
                {
                    LogLines = lines
                };

                var result = _client.PostAsJsonAsync("/api/logs/import", command).Result;
                result.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void Dispose()
        {
            _changable.Dispose();
            _client.Dispose();
        }

        protected virtual void OnChanged(ChangedEventArgs e)
        {
            Changed?.Invoke(this, e);
        }
    }
}