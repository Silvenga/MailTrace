namespace MailTrace.NetworkTail.Interfaces
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IHttpClient : IDisposable
    {
        Task<HttpResponseMessage> PostAsJsonAsync<T>(string api, T command);
        Uri BaseAddress { get; set; }
    }
}