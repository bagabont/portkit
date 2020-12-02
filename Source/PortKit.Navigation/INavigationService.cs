using System;
using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public interface INavigationService
    {
        bool CanPop();

        Task PopAsync(object parameter = null, bool animated = true);

        Task PushModalAsync(Type pageKey, object parameter = null, bool animated = true);

        Task PushAsync(Type pageKey, PushMode pushMode, object parameter = null, bool animated = true);

        Task PushAsync(Type[] pageKeys, PushMode pushMode, object parameter = null, bool animated = true);
    }
}