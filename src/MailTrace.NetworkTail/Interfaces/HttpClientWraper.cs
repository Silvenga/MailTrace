namespace MailTrace.NetworkTail.Interfaces
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    public class HttpClientWraper : IHttpClient
    {
        private readonly System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient();

        public Uri BaseAddress
        {
            get { return _client.BaseAddress; }
            set { _client.BaseAddress = value; }
        }

        public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string api, T command)
        {
            return await _client.PostAsJsonAsync(api, command);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}