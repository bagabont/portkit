using System;

namespace PortKit.MVVM
{
    public interface IUIDispatcher
    {
        bool CheckAccess();

        void Invoke(Action action);
    }
}