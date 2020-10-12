using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using PortKit.Extensions;

namespace PortKit.MVVM
{
    public class RelayCommand<T> : ICommand, IDisposable
    {
        public event EventHandler CanExecuteChanged;

        private readonly Dictionary<string, DisposableAction> _subscriptions =
            new Dictionary<string, DisposableAction>();

        private readonly Action<T> _action;
        private readonly Func<T, bool> _canExecute;

        public RelayCommand(Action<T> action, Func<T, bool> canExecute = null)
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
            return _canExecute == null || _canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            _action((T) parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public RelayCommand<T> Watch<TProperty>(Expression<Func<TProperty>> expression)
        {
            var caller = expression.GetCaller();
            var root = BindingLink.FromExpression(expression, caller, RaiseCanExecuteChanged)
                .First();

            root.Bind(caller).Evaluate(true);

            _subscriptions.Add(expression.ToString(), new DisposableAction(root.Dispose));

            return this;
        }

        public RelayCommand<T> Unwatch<TProperty>(Expression<Func<TProperty>> expression)
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