using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PortKit.Crypto
{
    public interface ICipher
    {
        Task EncryptAsync(byte[] key, Stream source, Stream destination, CancellationToken ct);

        Task DecryptAsync(byte[] key, Stream source, Stream destination, CancellationToken ct);
    }
}