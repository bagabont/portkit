using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public static class NavigationHelper
    {
        public static Task OnNavigatingAsync(NavigationDirection direction, INavigationAware fromContext, INavigationAware toContext, object parameter)
        {
            return Task.Run(() =>
            {
                var tasks = new List<Task>();

                if (fromContext != null)
                {
                    tasks.Add(fromContext.OnDisappearingAsync(direction, parameter));
                }

                if (toContext != null)
                {
                    tasks.Add(toContext.OnAppearingAsync(direction, parameter));
                }

                return Task.WhenAll(tasks);
            });
        }

        public static Task OnNavigatedAsync(NavigationDirection direction, INavigationAware fromContext, INavigationAware toContext, object parameter)
        {
            return Task.Run(() =>
            {
                var tasks = new List<Task>();

                if (fromContext != null)
                {
                    tasks.Add(fromContext.OnDisappearedAsync(direction, parameter));
                }

                if (toContext != null)
                {
                    tasks.Add(toContext.OnAppearedAsync(direction, parameter));
                }

                return Task.WhenAll(tasks);
            });
        }
    }
}