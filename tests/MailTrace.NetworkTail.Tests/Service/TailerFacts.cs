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
        private readonly string _path;

        private FileStream _fileStream;
        private StreamWriter _streamWriter;

        public TailerFacts()
        {
            _path = Path.GetTempFileName();
            CreateTempFile();
        }

        private void CreateTempFile()
        {
            _fileStream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
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

            var tailer = new ChangeTailer(_path);
            tailer.Changed += (sender, s) =>
            {
                result = s.Value;
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

            var tailer = new ChangeTailer(_path)
            {
                PollInterval = 1
            };
            tailer.Changed += (sender, s) =>
            {
                result = s.Value;
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
        public void When_file_is_replaced_and_is_smaller_tail_it_from_begining()
        {
            var content = AutoFixture.Create<string>();
            _streamWriter.Write(AutoFixture.Create<string>() + AutoFixture.Create<string>());

            var result = "";

            var tailer = new ChangeTailer(_path)
            {
                PollInterval = 1
            };
            tailer.Changed += (sender, s) =>
            {
                result = s.Value;
                _waiter.Set();
            };

            // Act
            tailer.Start();
            _waiter.WaitOne(1000).Should().BeTrue();

            Dispose();
            CreateTempFile();

            _streamWriter.Write(content);
            _waiter.WaitOne(1000).Should().BeTrue();

            // Assert
            result.Should().Be(content);
        }

        [Fact]
        public void When_file_is_replaced_and_is_same_size_tail_it_from_last_seek()
        {
            var content = AutoFixture.Create<string>();
            _streamWriter.Write(AutoFixture.Create<string>());

            var result = "";

            var tailer = new ChangeTailer(_path)
            {
                PollInterval = 1
            };
            tailer.Changed += (sender, s) =>
            {
                result = s.Value;
                _waiter.Set();
            };

            // Act
            tailer.Start();
            _waiter.WaitOne(1000).Should().BeTrue();

            Dispose();
            CreateTempFile();

            _streamWriter.Write(AutoFixture.Create<string>());
            _streamWriter.Write(content);
            _waiter.WaitOne(1000).Should().BeTrue();

            // Assert
            result.Should().Be(content);
        }

        [Fact]
        public void When_tailing_file_is_deleted_dont_throw()
        {
            var tailer = new ChangeTailer(_path)
            {
                PollInterval = 1
            };
            tailer.Start();
            _fileStream.Dispose();

            // Act
            File.Delete(_path);

            Thread.Sleep(10);

            // Assert
        }

        [Fact]
        public void When_disposing_tailer_dispose_of_file_stream()
        {
            var tailer = new ChangeTailer(_path)
            {
                PollInterval = 1
            };
            tailer.Start();

            Thread.Sleep(10);

            // Act
            using (tailer)
            {
            }

            // Assert
            tailer.IsRunning.Should().BeFalse();
        }

        [Fact]
        public void When_already_started_starting_again_should_throw()
        {
            var tailer = new ChangeTailer(_path);
            tailer.Start();

            Thread.Sleep(10);

            // Act
            Action task = () => tailer.Start();

            // Assert
            task.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void When_started_stopping_should_stop_running()
        {
            var tailer = new ChangeTailer(_path);
            tailer.Start();

            // Act
            tailer.Stop();

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