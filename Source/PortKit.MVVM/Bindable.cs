using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PortKit.MVVM
{
    public abstract class Bindable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            Dispatch(() => handler.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        protected virtual bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            return Set(ref storage, value, default, propertyName);
        }

        protected virtual bool Set<T>(ref T storage, T value, IEqualityComparer<T> equalityComparer,
            [CallerMemberName] string propertyName = null)
        {
            if ((equalityComparer ?? EqualityComparer<T>.Default).Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected virtual void Dispatch(Action action)
        {
            var dispatcher = Platform.Shared?.Dispatcher;
            if (dispatcher?.CheckAccess() == false)
            {
                dispatcher.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}