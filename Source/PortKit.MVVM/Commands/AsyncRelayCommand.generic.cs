using System;
using System.Threading.Tasks;

namespace PortKit.MVVM.Commands
{
    public class AsyncRelayCommand<T> : BaseRelayCommand, IAsyncRelayCommand
    {
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private bool _isExecuting;

        public bool IsExecuting
        {
            get => _isExecuting;
            private set
            {
                if (_isExecuting == value)
                {
                    return;
                }

                _isExecuting = value;
                InvokeCanExecuteChanged();
            }
        }

        public AsyncRelayCommand(Func<T, Task> execute, Func<T, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute ?? (_ => true);
        }

        public async Task ExecuteAsync(object parameter)
        {
            var val = parameter;

            if (parameter != null && parameter.GetType() != typeof(T) && parameter is IConvertible)
            {
                val = Convert.ChangeType(parameter, typeof(T), null);
            }

            if (CanExecute(val))
            {
                IsExecuting = true;

                try
                {
                    if (val == null)
                    {
                        if (typeof(T).IsValueType)
                        {
                            await _execute.Invoke(default(T));
                        }
                        else
                        {
                            await _execute.Invoke((T)val);
                        }
                    }
                    else
                    {
                        await _execute.Invoke((T)val);
                    }
                }
                finally
                {
                    IsExecuting = false;
                }
            }
        }

        public override bool CanExecute(object parameter)
        {
            if (IsExecuting)
            {
                return false;
            }

            if (_canExecute == null)
            {
                return true;
            }

            if (parameter == null && typeof(T).IsValueType)
            {
                return _canExecute.Invoke(default);
            }

            if (parameter == null || parameter is T)
            {
                return _canExecute.Invoke((T)parameter);
            }

            return false;
        }

        public override async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }
    }
}