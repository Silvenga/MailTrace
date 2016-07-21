namespace MailTrace.Host.Tests.Fixtures
{
    using System.Data.Common;

    using MailTrace.Data;

    public class TestTraceContext : TraceContext
    {
        public TestTraceContext(DbConnection connection) : base(connection)
        {
        }
    }
}