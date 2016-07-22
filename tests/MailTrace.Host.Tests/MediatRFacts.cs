namespace MailTrace.Host.Tests
{
    using global::Ninject;

    using MailTrace.Host.Ninject;
    using MailTrace.Host.Queries;
    using MailTrace.Host.Queries.Logs;

    using MediatR;

    using Xunit;

    public class MediatrFacts
    {
        [Fact(Skip = "Not working")]
        public void Can_hookup_Mediatr()
        {
            // Act

            var kernel = KernelConfiguration.CreateKernel();
            
            var m = kernel.Get<IMediator>();

            var query = new ListLogs.Query();

            var result = m.Send(query);

            // Assert
        }
    }
}