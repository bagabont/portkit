using System;

namespace PortKit.MVVM
{
    /// <summary>
    /// 
    /// </summary>
    public interface IUIDispatcher
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool CheckAccess();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        void Invoke(Action action);
    }
}