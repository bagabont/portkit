using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public static class NavigationExtensions
    {
        public static Task PushModalAsync<TPage>(this INavigationService navigationService, object parameter = null, bool animated = true)
        {
            return navigationService.PushModalAsync(typeof(TPage), parameter, animated);
        }

        public static Task PushAsync<TPage>(this INavigationService navigationService, PushMode pushMode, object parameter = null, bool animated = true)
        {
            return navigationService.PushAsync(typeof(TPage), pushMode, parameter, animated);
        }

        public static void Destroy(this IEnumerable<IDestroyable> destroyables)
        {
            foreach (var destroyable in destroyables)
            {
                destroyable.Destroy();
            }
        }

        public static void DisposeContext(this IEnumerable<IPage> pages)
        {
            pages?.Select(x => x.DataContext)
                .OfType<IDisposable>().ToList()
                .ForEach(x => x?.Dispose());
        }
    }
}