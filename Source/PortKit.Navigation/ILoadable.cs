using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public interface ILoadable
    {
        Task LoadingAsync(Mode mode, object parameter);

        Task LoadedAsync(Mode mode, object parameter);

        Task UnloadingAsync(Mode mode, object parameter);

        Task UnloadedAsync(Mode mode, object parameter);
    }
}