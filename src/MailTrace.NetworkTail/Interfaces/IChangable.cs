namespace MailTrace.NetworkTail.Interfaces
{
    using System;

    using MailTrace.NetworkTail.Models;

    public interface IChangable : IDisposable
    {
        event EventHandler<ChangedEventArgs> Changed;
    }
}