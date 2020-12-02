using System;

namespace PortKit.MVVM.Commands
{
    /// <inheritdoc cref="IRelayCommand"/>>
    public class RelayCommand : RelayCommand<object>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="action">Action to be invoked by the command.</param>
        /// <param name="canExecute">Predicate which checks if the command's action can be invoked.</param>
        public RelayCommand(Action action, Func<bool> canExecute)
            : base(_ => action?.Invoke(), _ => canExecute?.Invoke() ?? true)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="RelayCommand"/> class
        /// which has an always true invocation predicate.
        /// </summary>
        /// <param name="action">Action to be invoked by the command.</param>
        public RelayCommand(Action action)
            : this(action, () => true)
        {
        }
    }
}