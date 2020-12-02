using System;

namespace PortKit.MVVM.Commands
{
    /// <inheritdoc cref="IRelayCommand"/>>
    /// <typeparam name="TParameter">Type of the command parameter.</typeparam>
    public class RelayCommand<TParameter> : BaseRelayCommand
    {
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

        /// <summary>
        /// Checks if the command's action can be invoked.
        /// </summary>
        /// <param name="parameter">Parameter passed to the invocation predicated.</param>
        /// <returns>True if the action can be invoked, otherwise false.</returns>
        public override bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((TParameter)parameter);
        }

        /// <summary>
        /// Executes the command's action.
        /// </summary>
        /// <param name="parameter">Parameter passed to the invoked action.</param>
        public override void Execute(object parameter)
        {
            _action((TParameter)parameter);
        }
    }
}