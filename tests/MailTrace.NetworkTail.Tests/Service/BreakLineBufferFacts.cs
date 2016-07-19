namespace MailTrace.NetworkTail.Tests.Service
{
    using System;
    using System.Threading;

    using FluentAssertions;

    using MailTrace.NetworkTail.Interfaces;
    using MailTrace.NetworkTail.Models;
    using MailTrace.NetworkTail.Service;

    using NSubstitute;

    using Ploeh.AutoFixture;

    using Xunit;

    public class BreakLineBufferFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();
        private readonly EventWaitHandle _waiter = new EventWaitHandle(false, EventResetMode.AutoReset);

        [Fact]
        public void When_receiving_incomplete_lines_wait()
        {
            var mockChangable = Substitute.For<IChangable>();
            var buffer = new BreakLineBuffer(mockChangable);

            var one = AutoFixture.Create<string>();
            var two = AutoFixture.Create<string>();

            buffer.Changed += (sender, s) => { _waiter.Set(); };

            // Act
            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(one));
            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(two));

            // Assert
            _waiter.WaitOne(100).Should().BeFalse();
        }

        [Fact]
        public void When_receiving_complete_lines_return()
        {
            var mockChangable = Substitute.For<IChangable>();
            var buffer = new BreakLineBuffer(mockChangable);

            var one = AutoFixture.Create<string>() + Environment.NewLine;

            string result = null;
            buffer.Changed += (sender, s) =>
            {
                result = s.Value;
                _waiter.Set();
            };

            // Act
            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(one));

            _waiter.WaitOne(1000);

            // Assert
            result.Should().Be(one);
        }

        [Fact]
        public void When_receiving_complete_line_after_partial_return_concat()
        {
            var mockChangable = Substitute.For<IChangable>();
            var buffer = new BreakLineBuffer(mockChangable);

            var one = AutoFixture.Create<string>();
            var two = AutoFixture.Create<string>() + Environment.NewLine;

            string result = null;
            buffer.Changed += (sender, s) =>
            {
                result = s.Value;
                _waiter.Set();
            };

            // Act
            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(one));
            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(two));

            _waiter.WaitOne(1000);

            // Assert
            result.Should().Be(one + two);
        }

        [Fact]
        public void When_receiving_complete_line_with_partial_return_truncate()
        {
            var mockChangable = Substitute.For<IChangable>();
            var buffer = new BreakLineBuffer(mockChangable);

            var one = AutoFixture.Create<string>() + Environment.NewLine;
            var two = AutoFixture.Create<string>();

            string result = null;
            buffer.Changed += (sender, s) =>
            {
                result = s.Value;
                _waiter.Set();
            };

            // Act
            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(one + two));

            _waiter.WaitOne(1000);

            // Assert
            result.Should().Be(one);
        }
    }
}