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
        /// <param name="field">Backing field of the property.</param>
        /// <param name="value">New property value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <returns>True if the property value was changed, otherwise false.</returns>
        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            return Set(ref field, value, default, propertyName);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event if the property value has changed.
        /// </summary>
        /// <param name="field">Backing field of the property.</param>
        /// <param name="value">New property value.</param>
        /// <param name="equalityComparer"><see cref="IEqualityComparer{T}"/> to check if the property values is the same
        /// as the new value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <returns>True if the property value was changed, otherwise false.</returns>
        protected virtual bool Set<T>(ref T field, T value, IEqualityComparer<T> equalityComparer,
            [CallerMemberName] string propertyName = null)
        {
            if ((equalityComparer ?? EqualityComparer<T>.Default).Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event if the model's property value has changed.
        /// </summary>
        /// <param name="currentValue">Current property value of the model.</param>
        /// <param name="value">New property value.</param>
        /// <param name="model">The model for which the property is set.</param>
        /// <param name="updateAction">Action which updates the model's property, if it has changed.</param>
        /// <param name="equalityComparer"><see cref="IEqualityComparer{T}"/> to check if the property values is the same
        /// as the new value.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <returns>True if the property value was changed, otherwise false.</returns>
        protected virtual bool Set<TModel, T>(
            T currentValue,
            T value,
            TModel model,
            Action<TModel, T> updateAction,
            IEqualityComparer<T> equalityComparer,
            [CallerMemberName] string propertyName = null)
            where TModel : class
        {
            if ((equalityComparer ?? EqualityComparer<T>.Default).Equals(currentValue, value))
            {
                return false;
            }

            updateAction(model, value);

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