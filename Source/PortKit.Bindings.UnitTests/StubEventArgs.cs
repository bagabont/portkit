using System;

namespace PortKit.Bindings.UnitTests
{
    public sealed class StubEventArgs : EventArgs
    {
        public object Data { get; }

        public StubEventArgs(object data)
        {
            Data = data;
        }
    }
}