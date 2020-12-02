using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public interface ILifecycleAware
    {
        Task ResumeAsync();

        Task SuspendAsync();
    }
}