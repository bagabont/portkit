using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortKit.Navigation
{
    public sealed class Navigation
    {
        private readonly ViewFactory _viewFactory;
        private readonly List<IView> _views;

        public Navigation(ViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
            _views = new List<IView>();
        }

        public async Task OpenAsync<TView>(object parameter)
            where TView : IView
        {
            var nextView = _viewFactory.ResolveView<TView>();
            nextView.Dismissed += OnDismissed;

            var nextPresenter = _viewFactory.ResolvePresenter<TView>();
            var currentView = _views.LastOrDefault();
            _views.Add(nextView);

            await nextPresenter.PresentAsync(nextView);

            await NavigatingAsync(Mode.Forward, currentView, nextView, parameter).ConfigureAwait(false);
            await NavigatedAsync(Mode.Forward, currentView, nextView, parameter).ConfigureAwait(false);
        }

        private async void OnDismissed(object sender, EventArgs e)
        {
            await DismissAsync(default, true);
        }

        public async Task CloseAsync(object parameter)
        {
            await DismissAsync(parameter, false);
        }

        private async Task DismissAsync(object parameter, bool isSystem)
        {
            var currentView = _views.LastOrDefault();
            if (currentView != null)
            {
                if (isSystem)
                {
                    var currentPresenter = _viewFactory.ResolvePresenter(currentView.GetType());
                    await currentPresenter.UnPresentAsync(currentView);
                }

                _views.Remove(currentView);
                currentView.Dismissed -= OnDismissed;
            }

            var previousView = _views.LastOrDefault();

            await NavigatingAsync(Mode.Backward, currentView, previousView, parameter).ConfigureAwait(false);
            await NavigatedAsync(Mode.Backward, currentView, previousView, parameter).ConfigureAwait(false);
        }

        private static Task NavigatingAsync(Mode mode, IView fromView, IView toView, object parameter)
        {
            return Task.Run(() =>
            {
                var tasks = new List<Task>();

                if (fromView?.DataContext is ILoadable fromContext)
                {
                    tasks.Add(fromContext.UnloadingAsync(mode, parameter));
                }

                if (toView?.DataContext is ILoadable toContext)
                {
                    tasks.Add(toContext.LoadingAsync(mode, parameter));
                }

                return Task.WhenAll(tasks);
            });
        }

        private static Task NavigatedAsync(Mode mode, IView fromView, IView toView, object parameter)
        {
            return Task.Run(() =>
            {
                var tasks = new List<Task>();

                if (fromView?.DataContext is ILoadable fromContext)
                {
                    tasks.Add(fromContext.UnloadedAsync(mode, parameter));
                }

                if (toView?.DataContext is ILoadable toContext)
                {
                    tasks.Add(toContext.LoadedAsync(mode, parameter));
                }

                return Task.WhenAll(tasks);
            });
        }
    }
}