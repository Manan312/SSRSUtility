using Core.Entities;

namespace Core.Interfaces
{
    public interface ISSRSClientFactory
    {
        public HttpClient Create(SsrsConnectionInfo connection);
        public HttpClient CreateSoapClient(SsrsConnectionInfo connection);
    }
}
