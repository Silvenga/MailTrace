namespace MailTrace.NetworkTail.Tests.Service
{
    using System;
    using System.IO;
    using System.Threading;

    using FluentAssertions;

    using MailTrace.NetworkTail.Service;

    using Ploeh.AutoFixture;

    using Xunit;

    public class TailerFacts : IDisposable
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly EventWaitHandle _waiter = new EventWaitHandle(false, EventResetMode.AutoReset);

        private readonly FileStream _fileStream;
        private readonly StreamWriter _streamWriter;
        private readonly string _path;

        public TailerFacts()
        {
            _path = Path.GetTempFileName();
            _fileStream = File.Open(_path, FileMode.Open, FileAccess.ReadWrite, FileShare.Delete | FileShare.ReadWrite);
            _streamWriter = new StreamWriter(_fileStream)
            {
                AutoFlush = true
            };
        }

        [Fact]
        public void When_data_exists_on_start_tailer_fires_changed()
        {
            var content = AutoFixture.Create<string>();
            _streamWriter.Write(content);

            var result = "";

            var tailer = new Tailer(_path);
            tailer.Change += (sender, s) =>
            {
                result = s;
                _waiter.Set();
            };

            // Act
            tailer.Start();
            _waiter.WaitOne(1000);

            // Assert
            result.Should().Be(content);
        }

        [Fact]
        public void When_data_is_writen_tailer_fires_changed()
        {
            var content = AutoFixture.Create<string>();
            _streamWriter.Write(AutoFixture.Create<string>());

            var result = "";

            var tailer = new Tailer(_path)
            {
                PollInterval = 1
            };
            tailer.Change += (sender, s) =>
            {
                result = s;
                _waiter.Set();
            };

            // Act
            tailer.Start();
            _waiter.WaitOne(1000);
            _streamWriter.Write(content);
            _waiter.WaitOne(1000);

            // Assert
            result.Should().Be(content);
        }

        [Fact]
        public void When_tailing_file_is_deleted_dont_throw()
        {
            var tailer = new Tailer(_path)
            {
                PollInterval = 1
            };
            tailer.Start();
            _fileStream.Dispose();

            // Act
            File.Delete(_path);

            Thread.Sleep(100);

            // Assert
        }

        [Fact]
        public void When_disposing_tailer_dispose_of_file_stream()
        {
            var tailer = new Tailer(_path)
            {
                PollInterval = 1
            };
            tailer.Start();

            // Act
            using (tailer)
            {
            }

            // Assert
            tailer.IsRunning.Should().BeFalse();
        }

        public void Dispose()
        {
            try
            {
                _streamWriter.Dispose();
                if (File.Exists(_path))
                {
                    File.Delete(_path);
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}