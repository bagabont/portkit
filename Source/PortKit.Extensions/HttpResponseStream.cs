using System.IO;
using System.Net.Http;

namespace PortKit.Extensions
{
    public sealed class HttpResponseStream : ProxyStream
    {
        private readonly HttpResponseMessage _response;

        public HttpResponseStream(HttpResponseMessage response, Stream baseStream)
            : base(baseStream)
        {
            _response = response;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _response?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}