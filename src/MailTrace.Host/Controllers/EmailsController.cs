namespace MailTrace.Host.Controllers
{
    using System.Web.Http;

    using MailTrace.Host.Queries.Emails;

    using MediatR;

    [RoutePrefix("api/emails")]
    public class EmailsController : ApiController
    {
        private readonly IMediator _mediator;

        public EmailsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route(""), HttpGet]
        public IHttpActionResult ListEmailsAsync([FromUri] ListEmails.Query query)
        {
            query = query ?? new ListEmails.Query();

            var result = _mediator.Send(query);

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