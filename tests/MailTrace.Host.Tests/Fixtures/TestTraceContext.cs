namespace MailTrace.Host.Tests.Fixtures
{
    using System.Data.Common;

    using MailTrace.Host.Data;

    public class TestTraceContext : TraceContext
    {
        public TestTraceContext(DbConnection connection) : base(connection)
        {
        }
    }
}