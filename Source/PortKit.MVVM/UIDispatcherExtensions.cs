using System;
using System.Threading.Tasks;

namespace PortKit.MVVM
{
    public static class UIDispatcherExtensions
    {
        /// <summary>
        /// Dispatches an action asynchronously.
        /// </summary>
        /// <param name="dispatcher">Dispatcher which invokes the action.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <typeparam name="T">Returned value type</typeparam>
        /// <returns>Returned <see cref="Task{T}"/> result of the action.</returns>
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
        /// Dispatches an action asynchronously.
        /// </summary>
        /// <param name="dispatcher">Dispatcher which invokes the action.</param>
        /// <param name="action">Action to be invoked.</param>
        /// <returns>Returned <see cref="Task"/> of the action.</returns>
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