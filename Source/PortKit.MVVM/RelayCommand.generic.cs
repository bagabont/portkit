using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using PortKit.Extensions;

namespace PortKit.MVVM
{
    /// <inheritdoc cref="IRelayCommand"/>>
    /// <typeparam name="TParameter">Type of the command parameter.</typeparam>
    public class RelayCommand<TParameter> : Bindable, IRelayCommand
    {
        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>>
        public event EventHandler CanExecuteChanged;

        private readonly Dictionary<string, DisposableAction> _subscriptions = new Dictionary<string, DisposableAction>();
        private readonly Action<TParameter> _action;
        private readonly Func<TParameter, bool> _canExecute;

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand{T}"/> class.
        /// </summary>
        /// <param name="action">Action to be invoked by the command.</param>
        /// <param name="canExecute">Predicate which checks if the command's action can be invoked.</param>
        public RelayCommand(Action<TParameter> action, Func<TParameter, bool> canExecute)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand{T}"/> class
        /// which has an always true invocation predicate.
        /// </summary>
        /// <param name="action">Action to be invoked by the command.</param>
        public RelayCommand(Action<TParameter> action)
            : this(action, _ => true)
        {
        }

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

        /// <summary>
        /// Checks if the command's action can be invoked.
        /// </summary>
        /// <param name="parameter">Parameter passed to the invocation predicated.</param>
        /// <returns>True if the action can be invoked, otherwise false.</returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((TParameter)parameter);
        }

        /// <summary>
        /// Executes the command's action.
        /// </summary>
        /// <param name="parameter">Parameter passed to the invoked action.</param>
        public void Execute(object parameter)
        {
            _action((TParameter)parameter);
        }

        /// <inheritdoc cref="IRelayCommand.InvokeCanExecuteChanged"/>>
        public void InvokeCanExecuteChanged()
        {
            Dispatch(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
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
    }
}