using System;
using System.Security.Cryptography;

namespace PortKit.Crypto
{
    public sealed class CryptoRandom : ICryptoRandom
    {
        private static readonly RNGCryptoServiceProvider SecureRandom = new RNGCryptoServiceProvider();
        private static readonly Lazy<ICryptoRandom> LazyInstance = new Lazy<ICryptoRandom>(() => new CryptoRandom());

        public static ICryptoRandom SharedInstance => LazyInstance.Value;

        public byte[] GenerateBytes(int length)
        {
            var bytes = new byte[length];
            SecureRandom.GetBytes(bytes);
            return bytes;
        }
    }
}