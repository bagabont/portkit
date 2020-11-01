using System;
using System.Threading;

namespace PortKit.MVVM
{
    /// <inheritdoc cref="IUIDispatcher"/>
    public class SynchronizationContextDispatcher : IUIDispatcher
    {
        private static readonly Lazy<SynchronizationContextDispatcher> LazyShared =
            new Lazy<SynchronizationContextDispatcher>(() => new SynchronizationContextDispatcher());

        public static SynchronizationContextDispatcher Shared => LazyShared.Value;

        /// <summary>
        /// Attached <see cref="SynchronizationContext"/>.
        /// </summary>
        public SynchronizationContext Context { get; private set; }

        /// <summary>
        /// Uses the current <see cref="SynchronizationContext"/> as a dispatching context.
        /// </summary>
        public void Attach()
        {
            Attach(SynchronizationContext.Current);
        }

        /// <summary>
        /// Uses the provided <see cref="SynchronizationContext"/> as a dispatching context.
        /// </summary>
        /// <param name="context"><see cref="SynchronizationContext"/> to be used for dispatching.</param>
        public void Attach(SynchronizationContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc cref="IUIDispatcher.CheckAccess"/>
        public bool CheckAccess()
        {
            if (Context == null)
            {
                return false;
            }

            return SynchronizationContext.Current == Context;
        }

        /// <inheritdoc cref="IUIDispatcher.Invoke"/>
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