namespace PortKit.Crypto
{
    public interface ICryptoRandom
    {
        byte[] GenerateBytes(int length);
    }
}