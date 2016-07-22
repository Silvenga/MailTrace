namespace MailTrace.Host.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;

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

        //[Route("list"), HttpGet]
        //public IHttpActionResult ListLogsAsync([FromUri] ListLogs.Query query)
        //{
        //    query = query ?? new ListLogs.Query();

        //    var result = _mediator.Send(query);

        //    return Ok(result);
        //}

        [Route("import"), HttpPost]
        public async Task<IHttpActionResult> ImportAsync([FromBody] ImportLogs.Command command)
        {
            var result = await _mediator.SendAsync(command);
            return Ok(result);
        }
    }
}