using System;
using System.Windows.Input;
using PortKit.Extensions;

namespace PortKit.Bindings.Extensions
{
    public static class RelayCommandExtensions
    {
        public static IDisposable AttachEventHandler(this object element, string eventName, Delegate handler)
        {
            var eventInfo = element.GetType().GetEvent(eventName);
            if (eventInfo == null)
            {
                throw new ArgumentException($"Object of type '{element.GetType().FullName}' does not " +
                                            $"expose a public event called '{eventName}'", nameof(eventName));
            }

            eventInfo.AddEventHandler(element, handler);

            return new DisposableAction(() => eventInfo.RemoveEventHandler(element, handler));
        }

        public static IDisposable SetCommand(this object element,
            string eventName,
            ICommand command,
            Func<Event<EventArgs>, object> converter = null)
        {
            EventHandler handler = (s, e) =>
            {
                var parameter = converter?.Invoke(new Event<EventArgs>(s, e));
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            };

            return element.AttachEventHandler(eventName, handler);
        }

        public static IDisposable SetCommand<TEventArgs>(this object element,
            string eventName,
            ICommand command,
            Func<Event<TEventArgs>, object> converter = null)
        {
            EventHandler<TEventArgs> handler = (s, e) =>
            {
                var parameter = converter?.Invoke(new Event<TEventArgs>(s, e));
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            };

            return element.AttachEventHandler(eventName, handler);
        }

        public static IDisposable SetCommand<TDelegate, TEventArgs>(this object element,
            string eventName,
            ICommand command,
            Func<Event<TEventArgs>, object> converter = null)
        {
            void Handler(object s, TEventArgs e)
            {
                var parameter = converter?.Invoke(new Event<TEventArgs>(s, e));
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }

            var handler = (Delegate) (object) CreateDelegate<TDelegate, TEventArgs>(Handler);

            return element.AttachEventHandler(eventName, handler);
        }

        public static IDisposable SetCommand<TDelegate, TEventArgs>(this object element,
            ICommand command,
            Action<TDelegate> addHandler,
            Action<TDelegate> removeHandler,
            Func<Event<TEventArgs>, object> converter = null)
        {
            void Handler(object s, TEventArgs e)
            {
                var parameter = converter?.Invoke(new Event<TEventArgs>(s, e));
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }

            var handler = CreateDelegate<TDelegate, TEventArgs>(Handler);

            addHandler(handler);

            return new DisposableAction(() => removeHandler(handler));
        }

        public static IDisposable SetCommand<TEventArgs>(this object element,
            ICommand command,
            Action<EventHandler<TEventArgs>> addHandler,
            Action<EventHandler<TEventArgs>> removeHandler,
            Func<Event<TEventArgs>, object> converter = null)
        {
            void Handler(object s, TEventArgs e)
            {
                var parameter = converter?.Invoke(new Event<TEventArgs>(s, e));
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }

            addHandler(Handler);

            return new DisposableAction(() => removeHandler?.Invoke(Handler));
        }

        public static IDisposable SetCommand(this object element,
            ICommand command,
            Action<EventHandler> addHandler,
            Action<EventHandler> removeHandler,
            Func<Event<EventArgs>, object> converter = null)
        {
            void Handler(object s, EventArgs e)
            {
                var parameter = converter?.Invoke(new Event<EventArgs>(s, e));
                if (command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }

            addHandler(Handler);

            return new DisposableAction(() => removeHandler(Handler));
        }

        private static TDelegate CreateDelegate<TDelegate, TEventArgs>(Action<object, TEventArgs> handle)
        {
            var handleType = typeof(Action<object, TEventArgs>);
            var methodInfo = handleType.GetMethod(nameof(Action.Invoke)) ??
                             throw new MissingMethodException(handleType.Name, nameof(Action.Invoke));

            return (TDelegate) (object) Delegate.CreateDelegate(typeof(TDelegate), handle, methodInfo);
        }
    }
}