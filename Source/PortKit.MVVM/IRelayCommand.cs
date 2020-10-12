using System;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PortKit.MVVM
{
    public interface IRelayCommand : ICommand, IDisposable
    {
        IRelayCommand Watch<TProperty>(Expression<Func<TProperty>> expression);

        IRelayCommand Unwatch<TProperty>(Expression<Func<TProperty>> expression);

        void RaiseCanExecuteChanged();
    }
}