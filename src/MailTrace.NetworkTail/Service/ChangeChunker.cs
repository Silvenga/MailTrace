namespace MailTrace.NetworkTail.Service
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Timers;

    using MailTrace.NetworkTail.Interfaces;
    using MailTrace.NetworkTail.Models;

    public class ChangeChunker : IChangable
    {
        private readonly IChangable _changable;
        private readonly Timer _timer;

        private readonly BlockingCollection<string> _queue = new BlockingCollection<string>();

        public int MinimalInterval { get; set; } = 5000;

        public event EventHandler<ChangedEventArgs> Changed;

        public ChangeChunker(IChangable changable)
        {
            _changable = changable;
            _changable.Changed += ChangableOnChanged;

            _timer = new Timer
            {
                Interval = MinimalInterval,
                AutoReset = true,
                Enabled = false
            };

            _timer.Elapsed += (sender, args) => SendQueue();
        }

        private void SendQueue()
        {
            _timer.Stop();
            var message = string.Join("", _queue.GetConsumingEnumerable().Take(_queue.Count));
            OnChanged(message);
        }

        private void ChangableOnChanged(object sender, ChangedEventArgs args)
        {
            _queue.Add(args.Value);
            _timer.Start();
        }

        protected virtual void OnChanged(string e)
        {
            Changed?.Invoke(this, new ChangedEventArgs(e));
        }

        public void Dispose()
        {
            _changable.Dispose();
            _timer.Dispose();
        }
    }
}