using System;
using System.Collections.Generic;

namespace PortKit.Extensions
{
    public static class DisposableExtensions
    {
        public static IDisposable AddTo(this IDisposable disposable,
            ICollection<IDisposable> collection)
        {
            collection.Add(disposable);

            return disposable;
        }
    }
}