namespace PortKit.Bindings
{
    public sealed class Event<TEventArgs>
    {
        public object Sender { get; }

        public TEventArgs Args { get; }

        public Event(object sender, TEventArgs args)
        {
            Sender = sender;
            Args = args;
        }
    }
}