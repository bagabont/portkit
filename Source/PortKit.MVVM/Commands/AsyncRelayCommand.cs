using System;
using System.Threading.Tasks;

namespace PortKit.MVVM.Commands
{
    public class AsyncRelayCommand : AsyncRelayCommand<object>
    {
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
            : base(_ => execute.Invoke(), _ => canExecute?.Invoke() ?? true)
        {
        }
    }
}