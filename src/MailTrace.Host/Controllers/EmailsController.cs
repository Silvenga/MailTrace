namespace MailTrace.Host.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Http;

    using MailTrace.Host.Queries.Emails;

    using MediatR;

    using NLog;

    [RoutePrefix("api/emails")]
    public class EmailsController : ApiController
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;

        public EmailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route(""), HttpGet]
        public async Task<IHttpActionResult> ListEmailsAsync([FromUri] ListEmails.Query query)
        {
            query = query ?? new ListEmails.Query();

            var result = await _mediator.SendAsync(query);

            return Ok(result);
        }

        [Route("{messageId}"), HttpGet]
        public IHttpActionResult GetEmailAsync([FromUri] string messageId)
        {
            var query = new GetEmail.Query
            {
                MessageId = messageId
            };

            var result = _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}