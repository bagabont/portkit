using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PortKit.Extensions
{
    public static class StreamExtensions
    {
        public static async Task CopyToAsync(this Stream source, Stream destination, CancellationToken ct, int bufferSize, IProgress<int> bytesReadProgress)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (!source.CanRead)
            {
                throw new ArgumentException("Has to be readable", nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (!destination.CanWrite)
            {
                throw new ArgumentException("Has to be writable", nameof(destination));
            }

            if (bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize));
            }

            var buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, ct).ConfigureAwait(false)) != 0)
            {
                await destination.WriteAsync(buffer, 0, bytesRead, ct).ConfigureAwait(false);
                bytesReadProgress?.Report(bytesRead);
            }
        }
    }
}