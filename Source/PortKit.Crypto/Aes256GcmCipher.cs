using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.IO;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;

namespace PortKit.Crypto
{
    public sealed class Aes256GcmCipher : ICipher
    {
        private const int DefaultCopyBufferSize = 80 * 1024;
        private static readonly byte[] CryptoStreamHeader = Encoding.UTF8.GetBytes("PCS_v1.00");
        private readonly ICryptoRandom _cryptoRandom;

        public Aes256GcmCipher(ICryptoRandom cryptoRandom)
        {
            _cryptoRandom = cryptoRandom ?? throw new ArgumentNullException(nameof(cryptoRandom));
        }

        public async Task EncryptAsync(byte[] key, Stream source, Stream destination, CancellationToken ct)
        {
            await using var bw = new BinaryWriter(destination, Encoding.UTF8, true);
            bw.Write(CryptoStreamHeader);

            var iv = _cryptoRandom.GenerateBytes(16);
            bw.Write(iv.Length);
            bw.Write(iv, 0, iv.Length);

            var cipher = CreateCipher(key, iv, true);

            await CryptoTransformAsync(source, destination, cipher, ct).ConfigureAwait(false);
        }

        public async Task DecryptAsync(byte[] key, Stream source, Stream destination, CancellationToken ct)
        {
            using var br = new BinaryReader(source);
            var cryptoStreamHeader = br.ReadBytes(CryptoStreamHeader.Length);
            if (!cryptoStreamHeader.SequenceEqual(CryptoStreamHeader))
            {
                throw new ArgumentException(@"Invalid stream.", nameof(source));
            }

            var ivLength = br.ReadInt32();
            var iv = br.ReadBytes(ivLength);

            var cipher = CreateCipher(key, iv, false);

            await CryptoTransformAsync(source, destination, cipher, ct).ConfigureAwait(false);
        }

        private static async Task CryptoTransformAsync(Stream source, Stream destination, IBufferedCipher cipher, CancellationToken ct)
        {
            await using var cipherStream = new CipherStream(source, cipher, null);
            await cipherStream.CopyToAsync(destination, DefaultCopyBufferSize, ct).ConfigureAwait(false);
        }

        private static IBufferedCipher CreateCipher(byte[] key, byte[] iv, bool forEncryption)
        {
            var blockCipher = new GcmBlockCipher(new AesEngine());
            blockCipher.Init(forEncryption, new ParametersWithIV(new KeyParameter(key), iv));
            return new BufferedAeadBlockCipher(blockCipher);
        }
    }
}