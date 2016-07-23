namespace MailTrace.NetworkTail.Service
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using MailTrace.NetworkTail.Interfaces;
    using MailTrace.NetworkTail.Models;

    public class ChangeTailer : IChangable
    {
        private readonly string _filename;
        private long _currentSeek = -1;

        public event EventHandler<ChangedEventArgs> Changed;

        public bool IsRunning { get; private set; }
        public int PollInterval { get; set; } = 1000;

        public ChangeTailer(string filename)
        {
            _filename = filename;
        }

        public void Start()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException("Cannot start trailing once already started.");
            }
            Task.Run(() => { TailLoop(); });
        }

        public void Stop()
        {
            IsRunning = false;
        }

        private void TailLoop()
        {
            IsRunning = true;
            while (IsRunning)
            {
                Tail();
                Thread.Sleep(PollInterval);
            }
        }

        private void Tail()
        {
            string str;
            using (var fileStream = File.Open(_filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
            {
                var possibleSeek = fileStream.Length;
                if (_currentSeek == -1)
                {
                    _currentSeek = possibleSeek;
                }
                var seekDifference = possibleSeek - _currentSeek;

                if (seekDifference < 0)
                {
                    _currentSeek = 0;
                    return;
                }

                if (seekDifference == 0)
                {
                    return;
                }

                using (var memoryStream = new MemoryStream())
                using (var reader = new StreamReader(memoryStream))
                {
                    CopyStream(fileStream, memoryStream, _currentSeek, seekDifference);
                    memoryStream.Position = 0;
                    str = reader.ReadToEnd();
                }

                _currentSeek = possibleSeek;
            }

            OnChanged(str);
        }

        private static void CopyStream(Stream input, Stream output, long start, long length)
        {
            var bufferSize = Math.Min(4096, length);
            var buffer = new byte[bufferSize];
            long read;

            input.Seek(start, SeekOrigin.Begin);

            while ((read = input.Read(buffer, 0, (int) Math.Min(bufferSize, length))) > 0)
            {
                output.Write(buffer, 0, (int) read);
                length -= read;
            }
        }

        protected virtual void OnChanged(string e)
        {
            Changed?.Invoke(this, new ChangedEventArgs(e));
        }

        public void Dispose()
        {
            Stop();
        }
    }
}