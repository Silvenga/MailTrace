namespace MailTrace.NetworkTail.Service
{
    using System;

    using MailTrace.NetworkTail.Interfaces;
    using MailTrace.NetworkTail.Models;

    public class BreakLineBuffer : IChangable
    {
        private readonly IChangable _changable;
        public event EventHandler<ChangedEventArgs> Changed;

        private readonly object _rock = new object();

        private string _buffer = "";

        public BreakLineBuffer(IChangable changable)
        {
            _changable = changable;
            _changable.Changed += ChangableOnChanged;
        }

        private void ChangableOnChanged(object sender, ChangedEventArgs changedEventArgs)
        {
            string fullLines;
            try
            {
                lock (_rock)
                {
                    var str = _buffer + changedEventArgs.Value;
                    var lastFullLineEnd = str.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);

                    if (lastFullLineEnd == -1)
                    {
                        _buffer = str;
                        return;
                    }

                    fullLines = str.Substring(0, lastFullLineEnd + Environment.NewLine.Length);
                    var restOfStr = str.Substring(lastFullLineEnd + Environment.NewLine.Length);

                    _buffer = restOfStr;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            OnChanged(new ChangedEventArgs(fullLines));
        }

        public void Dispose()
        {
            _changable.Dispose();
        }

        protected virtual void OnChanged(ChangedEventArgs e)
        {
            Changed?.Invoke(this, e);
        }
    }
}