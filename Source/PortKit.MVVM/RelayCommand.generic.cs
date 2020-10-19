using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PortKit.Extensions;

namespace PortKit.MVVM
{
    /// <inheritdoc cref="IRelayCommand"/>>
    /// <typeparam name="TParameter">Type of the command parameter.</typeparam>
    public class RelayCommand<TParameter> : Bindable, IRelayCommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Dictionary<string, DisposableAction> _subscriptions = new Dictionary<string, DisposableAction>();
        private readonly Action<TParameter> _action;
        private readonly Func<TParameter, bool> _canExecute;

        public RelayCommand(Action<TParameter> action, Func<TParameter, bool> canExecute = null)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canExecute = canExecute;
        }

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

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((TParameter)parameter);
        }

        public void Execute(object parameter)
        {
            _action((TParameter)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            Dispatch(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        public IRelayCommand Watch<TProperty>(Expression<Func<TProperty>> expression)
        {
            var caller = expression.GetCaller();
            var rootObserver = PropertyObserver.FromExpression(expression, caller, RaiseCanExecuteChanged).First();

            rootObserver.Bind(caller).Evaluate(true);

            _subscriptions.Add(expression.ToString(), new DisposableAction(rootObserver.Dispose));

            return this;
        }

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
    }
}