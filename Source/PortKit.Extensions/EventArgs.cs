using System;

namespace PortKit.Extensions
{
    public class EventArgs<T> : EventArgs
    {
        public T Parameter { get; }

        public EventArgs(T parameter)
        {
            Parameter = parameter;
        }
    }
}