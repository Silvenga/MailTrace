namespace MailTrace.Host.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    using MailTrace.Host.Queries.Logs;

    using MediatR;

    using NLog;

    [RoutePrefix("api/logs")]
    public class LogsController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;

        public LogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("import"), HttpPost]
        public async Task<IHttpActionResult> ImportAsync([FromBody] ImportLogs.Command command)
        {
            var result = await _mediator.SendAsync(command);
            return Ok(result);
        }

        [Route("import/logstash"), HttpPost]
        public async Task<IHttpActionResult> ImportLogstashAsync([FromBody] LogstashMessage message)
        {
            var command = new ImportLogs.Command
            {
                LogLines = new List<string> {message.Message}
            };

            var result = await _mediator.SendAsync(command);
            return Ok(result);
        }
    }

    public class LogstashMessage
    {
        public string Message { get; set; }
    }
}