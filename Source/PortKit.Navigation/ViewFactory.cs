using System;
using System.Collections.Generic;

namespace PortKit.Navigation
{
    public class ViewFactory
    {
        private readonly Dictionary<Type, (Func<IView>, Func<IViewPresenter>)> _map;

        public ViewFactory()
        {
            _map = new Dictionary<Type, (Func<IView>, Func<IViewPresenter>)>();
        }

        public void Register<TView>(Func<IView> viewFactory, Func<IViewPresenter> presenterFactory)
            where TView : IView
        {
            _map.Add(typeof(TView), (viewFactory, presenterFactory));
        }

        public TView ResolveView<TView>()
            where TView : IView
        {
            return (TView)ResolveView(typeof(TView));
        }

        public IView ResolveView(Type viewType)
        {
            _map.TryGetValue(viewType, out var tuple);
            var (viewFactory, _) = tuple;

            return viewFactory();
        }

        public IViewPresenter ResolvePresenter<TView>()
            where TView : IView
        {
            return ResolvePresenter(typeof(TView));
        }

        public IViewPresenter ResolvePresenter(Type viewType)
        {
            _map.TryGetValue(viewType, out var tuple);
            var (_, presenterFactory) = tuple;

            return presenterFactory();
        }
    }
}