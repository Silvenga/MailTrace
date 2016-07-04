namespace MailTrace.NetworkTail.Service
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class Tailer : IDisposable
    {
        private readonly string _filename;
        public event EventHandler<string> Change;

        public long CurrentSeek { get; private set; }
        public bool IsRunning { get; private set; }

        public int PollInterval { get; set; } = 1000;

        public Tailer(string filename)
        {
            _filename = filename;
        }

        public void Start()
        {
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
                var seekDifference = possibleSeek - CurrentSeek;

                if (seekDifference < 0)
                {
                    CurrentSeek = 0;
                    return;
                }

                if (seekDifference == 0)
                {
                    return;
                }

                using (var memoryStream = new MemoryStream())
                using (var reader = new StreamReader(memoryStream))
                {
                    CopyStream(fileStream, memoryStream, CurrentSeek, seekDifference);
                    memoryStream.Position = 0;
                    str = reader.ReadToEnd();
                }

                CurrentSeek = possibleSeek;
            }

            OnChange(str);
        }

        private void CopyStream(Stream input, Stream output, long start, long length)
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

        protected virtual void OnChange(string e)
        {
            Change?.Invoke(this, e);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}