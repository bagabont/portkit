using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using PortKit.Extensions;

namespace PortKit.MVVM.Commands
{
    public abstract class BaseRelayCommand : Bindable, IRelayCommand
    {
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);

        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>>
        public event EventHandler CanExecuteChanged;

        private readonly Dictionary<string, DisposableAction> _subscriptions = new Dictionary<string, DisposableAction>();

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            foreach (var subscription in _subscriptions.Values)
            {
                subscription.Dispose();
            }

            _subscriptions.Clear();
        }

        /// <inheritdoc cref="IRelayCommand.Watch{TProperty}"/>>
        public IRelayCommand Watch<TProperty>(Expression<Func<TProperty>> expression)
        {
            var caller = expression.GetCaller();
            var rootObserver = PropertyObserver.FromExpression(expression, caller, InvokeCanExecuteChanged).First();

            rootObserver.Bind(caller).Evaluate(true);

            _subscriptions.Add(expression.ToString(), new DisposableAction(rootObserver.Dispose));

            return this;
        }

        /// <inheritdoc cref="IRelayCommand.Unwatch{TProperty}"/>>
        public IRelayCommand Unwatch<TProperty>(Expression<Func<TProperty>> expression)
        {
            var propertyChain = expression.ToString();
            if (!_subscriptions.TryGetValue(propertyChain, out var subscription))
            {
                return this;
            }

            subscription?.Dispose();
            _subscriptions.Remove(propertyChain);

            return this;
        }

        public void InvokeCanExecuteChanged()
        {
            Dispatch(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }
    }
}