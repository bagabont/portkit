using System;

namespace PortKit.Bindings.UnitTests.Data
{
    internal sealed class StubEventArgs : EventArgs
    {
        public object Data { get; }

        public StubEventArgs(object data)
        {
            Data = data;
        }
    }
}