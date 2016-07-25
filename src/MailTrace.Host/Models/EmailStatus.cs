namespace MailTrace.Host.Models
{
    public enum EmailStatus
    {
        // https://tools.ietf.org/html/rfc3463
        Success = 2,
        TransientFailure = 4,
        PermanentFailure = 5
    }
}