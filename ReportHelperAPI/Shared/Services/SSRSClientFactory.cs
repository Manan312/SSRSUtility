using Core.Entities;
using Core.Interfaces;
using System.Net;

namespace Infrastructure.Services
{
    public class SSRSClientFactory : ISSRSClientFactory
    {
        public HttpClient Create(SsrsConnectionInfo connection)
        {
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(
                    connection.Username,
                    connection.Password,
                    connection.Domain),
                PreAuthenticate = true
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(connection.BaseUrl)
            };
        }
        public HttpClient CreateSoapClient(SsrsConnectionInfo connection)
        {
            if (string.IsNullOrWhiteSpace(connection.BaseUrl))
                throw new ArgumentException("SSRS BaseUrl is required");

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(
                    connection.Username,
                    connection.Password,
                    connection.Domain),        // Domain can be null
                PreAuthenticate = true,
                UseDefaultCredentials = false
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri($"{connection.BaseUrl.TrimEnd('/')}/")
            };

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "text/xml");

            return client;
        }
    }
}
