using System;
using System.Threading.Tasks;

namespace PortKit.MVVM
{
    public static class UIDispatcherExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> InvokeAsync<T>(this IUIDispatcher dispatcher, Func<Task<T>> action)
        {
            var tcs = new TaskCompletionSource<T>();

            try
            {
                dispatcher.Invoke(async () => tcs.SetResult(await action()));
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            return await tcs.Task;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static async Task InvokeAsync(this IUIDispatcher dispatcher, Func<Task> action)
        {
            var tcs = new TaskCompletionSource<bool>();

            try
            {
                dispatcher.Invoke(async () =>
                {
                    await action();
                    tcs.SetResult(true);
                });
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }

            await tcs.Task;
        }
    }
}