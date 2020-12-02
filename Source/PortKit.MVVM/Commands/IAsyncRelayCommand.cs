using System.Threading.Tasks;

namespace PortKit.MVVM.Commands
{
    public interface IAsyncRelayCommand : IRelayCommand
    {
        /// <summary>
        /// Gets the state of the async command.
        /// </summary>
        bool IsExecuting { get; }

        /// <summary>
        /// Executes the asynchronous command.
        /// </summary>
        /// <param name="parameter">The parameter for the command.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ExecuteAsync(object parameter);
    }
}