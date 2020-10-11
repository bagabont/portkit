using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PortKit.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<Stream> GetStreamAsync(this HttpClient httpClient, string requestUrl, CancellationToken ct)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync()
                .ConfigureAwait(false);

            return new HttpResponseStream(response, stream);
        }

        public static async Task<byte[]> GetByteArrayAsync(this HttpClient httpClient, string requestUrl, CancellationToken ct)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var data = await response.Content.ReadAsByteArrayAsync()
                .ConfigureAwait(false);

            return data;
        }
    }
}