using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public interface IViewPresenter
    {
        Task PresentAsync(IView view);

        Task UnPresentAsync(IView view);
    }
}