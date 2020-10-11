using System;
using System.ComponentModel;

namespace PortKit.Bindings.UnitTests
{
    internal sealed class TestView
    {
        public event EventHandler NoArgumentsEvent;
        public event EventHandler<StubEventArgs> ArgumentsEvent;
        public event PropertyChangedEventHandler PropertyChangedEvent;
        internal event EventHandler InternalEvent;

        public void RaiseNoArgumentsEvent()
        {
            NoArgumentsEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RaiseArgumentsEvent(object data)
        {
            ArgumentsEvent?.Invoke(this, new StubEventArgs(data));
        }

        public void RaisePropertyChanged(string propName)
        {
            PropertyChangedEvent?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}