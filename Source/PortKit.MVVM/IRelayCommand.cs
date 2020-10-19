using System;
using System.Linq.Expressions;
using System.Windows.Input;

namespace PortKit.MVVM
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRelayCommand : ICommand, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        IRelayCommand Watch<TProperty>(Expression<Func<TProperty>> expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="TProperty"></typeparam>
        /// <returns></returns>
        IRelayCommand Unwatch<TProperty>(Expression<Func<TProperty>> expression);

        /// <summary>
        /// 
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}