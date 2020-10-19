using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PortKit.MVVM
{
    /// <summary>
    /// Provides an abstract implementation to use with data binding. The class
    /// enables also automatic dispatching of property updates to the UI thread.
    /// </summary>
    public abstract class Bindable : INotifyPropertyChanged
    {
        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            Dispatch(() => handler.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event if the property value has changed.
        /// </summary>
        /// <param name="storage">Backing field of the property.</param>
        /// <param name="value">New property value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <returns>True if the property value was changed and the <see cref="PropertyChanged"/> event was raised,
        /// otherwise false.
        /// </returns>
        protected virtual bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            return Set(ref storage, value, default, propertyName);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event if the property value has changed.
        /// </summary>
        /// <param name="storage">Backing field of the property.</param>
        /// <param name="value">New property value.</param>
        /// <param name="equalityComparer"><see cref="IEqualityComparer{T}"/> to check if the property values is the same
        /// as the new value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <returns>True if the property value was changed and the <see cref="PropertyChanged"/> event was raised,
        /// otherwise false.
        /// </returns>
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

        /// <summary>
        /// Checks if the current thread has access and invokes the action,
        /// if it does not, then the action is dispatched to the UI thread by the
        /// dispatcher.
        /// </summary>
        /// <param name="action">Action to be dispatched.</param>
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