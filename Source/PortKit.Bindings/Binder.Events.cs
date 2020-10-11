using System;
using System.Windows.Input;
using PortKit.Bindings.Extensions;
using PortKit.Extensions;

namespace PortKit.Bindings
{
    public sealed class Binder
    {
        public sealed class EventBinding<TE, TA>
        {
            private Action<TE> _addHandler;
            private Action<TE> _removeHandler;
            private Func<(object, TA), object> _converter;
            private Action<bool> _onCanExecuteChanged;

            public EventBinding<TE, TA> Subscribe(Action<TE> action)
            {
                _addHandler = action;
                return this;
            }

            public EventBinding<TE, TA> Unsubscribe(Action<TE> action)
            {
                _removeHandler = action;
                return this;
            }

            public EventBinding<TE, TA> Converter(Func<(object, TA), object> action)
            {
                _converter = action;
                return this;
            }

            public EventBinding<TE, TA> OnCanExecuteChanged(Action<bool> action)
            {
                _onCanExecuteChanged = action;
                return this;
            }

            public IDisposable Build(ICommand command)
            {
                void Execute(object s, TA e)
                {
                    command.Execute(_converter?.Invoke((s, e)));
                }

                var canExecuteChangedHandler = DelegateFactory.Create<EventHandler, EventArgs>(
                    (s, e) => _onCanExecuteChanged?.Invoke(false)
                );
                command.CanExecuteChanged += canExecuteChangedHandler;

                var handler = DelegateFactory.Create<TE, TA>(Execute);
                _addHandler(handler);

                return new DisposableAction(() =>
                {
                    command.CanExecuteChanged -= canExecuteChangedHandler;

                    _removeHandler(handler);
                });
            }
        }

        public static EventBinding<TEventHandler, TEventArgs> Event<TEventHandler, TEventArgs>() =>
            new EventBinding<TEventHandler, TEventArgs>();
    }
}