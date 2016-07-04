namespace MailTrace.NetworkTail.Service
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class Tailer : IDisposable
    {
        private readonly Stream _fileStream;
        public event EventHandler<string> Change;

        public long CurrentSeek { get; private set; }
        public bool Running { get; private set; }

        public Tailer(string filename)
        {
            _fileStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        }
        
        public void Start()
        {
            Task.Run(() => { TailLoop(); });
        }

        public void Stop()
        {
            Running = false;
        }

        private void TailLoop()
        {
            Running = true;
            while (Running)
            {
                Tail();
                Thread.Sleep(1000);
            }
        }

        private void Tail()
        {
            var possibleSeek = _fileStream.Length;
            var seekDifference = possibleSeek - CurrentSeek;
            CurrentSeek = possibleSeek;

            if (seekDifference <= 0)
            {
                return;
            }

            string str;

            using (var memoryStream = new MemoryStream())
            using (var reader = new StreamReader(memoryStream))
            {
                CopyStream(_fileStream, memoryStream, seekDifference);
                memoryStream.Position = 0;
                str = reader.ReadToEnd();
            }

            if (str == string.Empty)
            {
                return;
            }

            OnChange(str);
        }

        private void CopyStream(Stream input, Stream output, long count)
        {
            var bufferSize = Math.Min(4096, count);
            var buffer = new byte[bufferSize];
            long read;

            while ((read = input.Read(buffer, 0, (int) Math.Min(bufferSize, count))) > 0)
            {
                output.Write(buffer, 0, (int) read);
                count -= read;
            }
        }

        protected virtual void OnChange(string e)
        {
            Change?.Invoke(this, e);
        }

        public void Dispose()
        {
            _fileStream.Dispose();
        }
    }
}