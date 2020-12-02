using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PortKit.MVVM.Commands
{
    /// <summary>
    /// </summary>
    public interface IRelayCommand : ICommand, IDisposable
    {
        /// <summary>
        /// Observes the expression's property chain and invokes the <see cref="ICommand.CanExecuteChanged"/> when
        /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> or the 
        /// <see cref="INotifyCollectionChanged.CollectionChanged"/> event is raised.
        /// </summary>
        /// <param name="expression">Expression describing the property chain.</param>
        /// <typeparam name="TProperty">Type of the observed property.</typeparam>
        /// <returns>The <see cref="IRelayCommand"/> itself.</returns>
        IRelayCommand Watch<TProperty>(Expression<Func<TProperty>> expression);

        /// <summary>
        /// Stops observing the expression's property chain and invoking the <see cref="ICommand.CanExecuteChanged"/> when
        /// the <see cref="INotifyPropertyChanged.PropertyChanged"/> or the 
        /// <see cref="INotifyCollectionChanged.CollectionChanged"/> event is raised.
        /// </summary>
        /// <param name="expression">Expression describing the property chain.</param>
        /// <typeparam name="TProperty">Type of the observed property.</typeparam>
        /// <returns>The <see cref="IRelayCommand"/> itself.</returns>
        IRelayCommand Unwatch<TProperty>(Expression<Func<TProperty>> expression);

        /// <summary>
        /// Invokes the <see cref="ICommand.CanExecuteChanged"/> event.
        /// </summary>
        void InvokeCanExecuteChanged();
    }
}