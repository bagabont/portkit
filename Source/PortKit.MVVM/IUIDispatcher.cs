using System;

namespace PortKit.MVVM
{
    /// <summary>
    /// When implemented defines a dispatcher class which can be used to invoke actions on the UI thread.
    /// </summary>
    public interface IUIDispatcher
    {
        /// <summary>
        /// Checks if the dispatcher has access on the current thread.
        /// </summary>
        /// <returns>True if an action can be immediately invoked, false if the action has to be dispatched.</returns>
        bool CheckAccess();

        /// <summary>
        /// Invokes an action on the UI thread.
        /// </summary>
        /// <param name="action">Action to be invoked on the UI thread.</param>
        void Invoke(Action action);
    }
}