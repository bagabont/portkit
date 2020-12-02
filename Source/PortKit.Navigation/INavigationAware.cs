using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public interface INavigationAware
    {
        Task OnAppearingAsync(NavigationDirection direction, object parameter);

        Task OnDisappearingAsync(NavigationDirection direction, object parameter);

        Task OnAppearedAsync(NavigationDirection direction, object parameter);

        Task OnDisappearedAsync(NavigationDirection direction, object parameter);
    }
}