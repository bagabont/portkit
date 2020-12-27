namespace PortKit.Crypto
{
    public static class CryptoRandomExtensions
    {
        public const int KeySizeInBytes = 32;
        public const int IvSizeInBytes = 16;

        public static byte[] GenerateCryptoKey(this ICryptoRandom cryptoRandom)
        {
            return cryptoRandom.GenerateBytes(KeySizeInBytes);
        }

        public static byte[] GenerateCryptoIV(this ICryptoRandom cryptoRandom)
        {
            return cryptoRandom.GenerateBytes(IvSizeInBytes);
        }
    }
}