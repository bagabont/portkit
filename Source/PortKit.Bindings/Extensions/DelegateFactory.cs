using System;

namespace PortKit.Bindings.Extensions
{
    internal static class DelegateFactory
    {
        public static TDelegate Create<TDelegate, TEventArgs>(Action<object, TEventArgs> handle)
        {
            var handleType = typeof(Action<object, TEventArgs>);
            var methodInfo = handleType.GetMethod(nameof(Action.Invoke)) ??
                             throw new MissingMethodException(handleType.Name, nameof(Action.Invoke));

            return (TDelegate)(object)Delegate.CreateDelegate(typeof(TDelegate), handle, methodInfo);
        }
    }
}