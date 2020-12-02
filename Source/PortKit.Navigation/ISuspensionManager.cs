using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public interface ISuspensionManager
    {
        Task SuspendAsync();

        Task ResumeAsync();
    }
}