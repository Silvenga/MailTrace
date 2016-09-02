namespace MailTrace.Host.Tests.Data
{
    using System.Linq;

    using FluentAssertions;

    using MailTrace.Data.Entities;

    using Ploeh.AutoFixture;

    using Xunit;

    public class EmailFacts
    {
        public static readonly Fixture AutoFixture = new Fixture();

        [Fact]
        public void ToList_should_split_to_property()
        {
            var emailList = AutoFixture.CreateMany<string>().ToList();
            var emails = string.Join(";", emailList);

            var email = new Email {Recipient = emails};

            // Act
            var result = email.RecipientList;

            // Assert
            result.Should().BeEquivalentTo(emailList);
        }

        [Fact]
        public void When_to_is_null_return_empty_to_list()
        {
            var email = new Email {Recipient = null};

            // Act
            var result = email.RecipientList;

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void When_to_list_is_set_set_tos()
        {
            var emailList = AutoFixture.CreateMany<string>().ToList();

            // Act
            var email = new Email {RecipientList = emailList};

            // Assert
            var emails = string.Join(";", emailList);
            email.Recipient.Should().Be(emails);
        }
    }
}