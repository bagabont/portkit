using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PortKit.Crypto
{
    public static class CipherExtensions
    {
        public static async Task EncryptAsync(this ICipher cipher, byte[] key, Stream source, Stream destination)
        {
            await cipher.EncryptAsync(key, source, destination, CancellationToken.None)
                .ConfigureAwait(false);
        }

        public static async Task DecryptAsync(this ICipher cipher, byte[] key, Stream source, Stream destination)
        {
            await cipher.DecryptAsync(key, source, destination, CancellationToken.None)
                .ConfigureAwait(false);
        }

        public static async Task<byte[]> EncryptAsync(this ICipher cipher, byte[] key, byte[] clearData)
        {
            using (var source = new MemoryStream(clearData))
            using (var destination = new MemoryStream())
            {
                await cipher.EncryptAsync(key, source, destination).ConfigureAwait(false);
                return destination.ToArray();
            }
        }

        public static async Task<byte[]> DecryptAsync(this ICipher cipher, byte[] key, byte[] cipherData)
        {
            using (var source = new MemoryStream(cipherData))
            using (var destination = new MemoryStream())
            {
                await cipher.DecryptAsync(key, source, destination).ConfigureAwait(false);
                return destination.ToArray();
            }
        }

        public static async Task<string> EncryptAsync(this ICipher cipher, byte[] key, string clearText)
        {
            var clearData = Encoding.UTF8.GetBytes(clearText);
            var cipherData = await cipher.EncryptAsync(key, clearData).ConfigureAwait(false);
            return Convert.ToBase64String(cipherData);
        }

        public static async Task<string> DecryptAsync(this ICipher cipher, byte[] key, string cipherText)
        {
            var cipherData = Convert.FromBase64String(cipherText);
            var clearData = await cipher.DecryptAsync(key, cipherData).ConfigureAwait(false);
            return Encoding.UTF8.GetString(clearData);
        }
    }
}