namespace MailTrace.Components.Models.Logs
{
    public class PostfixService
    {
        public string Name { get; set; }

        public string Pid { get; set; }

        public override string ToString()
        {
            return $"{Name}({Pid})";
        }
    }
}