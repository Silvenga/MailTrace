namespace MailTrace.Components.Queries.Reports
{
    using System;

    using MailTrace.Data;

    using MediatR;

    public class DeliveryAttemptsReport
    {
        public class Query : IRequest<Result>
        {
        }

        public class Result
        {
        }
    }

    public class DeliveryAttemptsReportHandler : IRequestHandler<DeliveryAttemptsReport.Query, DeliveryAttemptsReport.Result>
    {
        private readonly TraceContext _context;

        public DeliveryAttemptsReportHandler(TraceContext context)
        {
            _context = context;
        }

        public DeliveryAttemptsReport.Result Handle(DeliveryAttemptsReport.Query message)
        {
            throw new NotImplementedException();
        }
    }
}