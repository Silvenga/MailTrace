namespace MailTrace.Host.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Results;

    using MailTrace.Host.Queries;

    using MediatR;

    [RoutePrefix("api/logs")]
    public class LogsController : ApiController
    {
        private readonly IMediator _mediator;

        public LogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("list"), HttpGet]
        public IHttpActionResult ListLogsAsync(ListLogs.Query query)
        {
            var result = _mediator.Send(query);

            return Ok(result);
        }
    }
}