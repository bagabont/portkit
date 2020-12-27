using System.Threading.Tasks;

namespace PortKit.Crypto
{
    public interface IKeyDerivator
    {
        Task<byte[]> DeriveAsync(byte[] password, byte[] iv, int rounds, int keySizeInBytes);
    }
}