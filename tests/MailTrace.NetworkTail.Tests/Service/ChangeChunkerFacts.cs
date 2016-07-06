namespace MailTrace.NetworkTail.Tests.Service
{
    using System.Threading;

    using FluentAssertions;

    using MailTrace.NetworkTail.Interfaces;
    using MailTrace.NetworkTail.Models;
    using MailTrace.NetworkTail.Service;

    using NSubstitute;

    using Ploeh.AutoFixture;

    using Xunit;

    public class ChangeChunkerFacts
    {
        private static readonly Fixture AutoFixture = new Fixture();

        private readonly EventWaitHandle _waiter = new EventWaitHandle(false, EventResetMode.AutoReset);

        [Fact]
        public void When_receiving_multiple_changes_chunk_changes()
        {
            var mockChangable = Substitute.For<IChangable>();
            var chunker = new ChangeChunker(mockChangable)
            {
                MinimalInterval = 10
            };

            var one = AutoFixture.Create<string>();
            var two = AutoFixture.Create<string>();

            string result = null;
            chunker.Changed += (sender, s) =>
            {
                result = s.Value;
                _waiter.Set();
            };

            // Act

            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(one));
            mockChangable.Changed += Raise.EventWith(new object(), new ChangedEventArgs(two));

            _waiter.WaitOne(1000 * 10).Should().BeTrue();

            // Assert
            result.Should().Be(one + two);
        }

        [Fact]
        public void When_chunker_is_disposed_changable_should_be_disposed()
        {
            var mockChangable = Substitute.For<IChangable>();
            var chunker = new ChangeChunker(mockChangable);

            // Act
            using (chunker)
            {
            }

            // Assert
            mockChangable.Received().Dispose();
        }
    }
}