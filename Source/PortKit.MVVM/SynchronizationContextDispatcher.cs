using System;
using System.Threading;

namespace PortKit.MVVM
{
    public class SynchronizationContextDispatcher : IUIDispatcher
    {
        private static readonly Lazy<SynchronizationContextDispatcher> LazyShared =
            new Lazy<SynchronizationContextDispatcher>(() => new SynchronizationContextDispatcher());

        public static SynchronizationContextDispatcher Shared => LazyShared.Value;

        public SynchronizationContext Context { get; private set; }

        public void Attach()
        {
            Attach(SynchronizationContext.Current);
        }

        public void Attach(SynchronizationContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public bool CheckAccess()
        {
            if (Context == null)
            {
                return false;
            }

            return SynchronizationContext.Current == Context;
        }

        public void Invoke(Action action)
        {
            if (Context == null)
            {
                Attach();
            }

            if (CheckAccess())
            {
                action?.Invoke();
            }
            else
            {
                Context.Post(_ => action?.Invoke(), null);
            }
        }
    }
}