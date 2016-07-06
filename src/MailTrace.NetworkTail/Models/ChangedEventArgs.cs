namespace MailTrace.NetworkTail.Models
{
    using System;

    public class ChangedEventArgs: EventArgs
    {
        public string Value { get; set; }

        public ChangedEventArgs(string value)
        {
            Value = value;
        }
    }
}
