using System;

namespace PortKit.MVVM
{
    public class RelayCommand : RelayCommand<object>
    {
        public RelayCommand(Action execute, Func<bool> canExecute = null)
            : base(_ => execute?.Invoke(), _ => canExecute?.Invoke() ?? true)
        {
        }
    }
}