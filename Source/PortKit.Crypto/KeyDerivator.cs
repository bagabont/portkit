using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PortKit.Crypto
{
    public sealed class KeyDerivator : IKeyDerivator
    {
        public async Task<byte[]> DeriveAsync(byte[] password, byte[] iv, int rounds, int keySizeInBytes)
        {
            return await Task.Run(() => Derive(password, iv, rounds, keySizeInBytes)).ConfigureAwait(false);
        }

        private static byte[] Derive(byte[] password, byte[] iv, int rounds, int keySizeInBytes)
        {
            using var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, iv, rounds);

            return rfc2898DeriveBytes.GetBytes(keySizeInBytes);
        }
    }
}