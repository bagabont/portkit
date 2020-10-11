using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace PortKit.MVVM
{
    public class BindableCollection<T> : ObservableCollection<T>
    {
        private readonly object _syncLock = new object();

        public BindableCollection()
        {
        }

        public BindableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }

        public virtual void Execute(Action<IList<T>> itemsAction)
        {
            lock (_syncLock)
            {
                itemsAction?.Invoke(Items);
                OnReset();
            }
        }

        public virtual void AddRange(IEnumerable<T> range)
        {
            CheckReentrancy();

            ((List<T>) Items).AddRange(range);

            OnReset();
        }

        public virtual void RemoveRange(IEnumerable<T> range)
        {
            CheckReentrancy();

            var removed = Items.Intersect(range).ToArray();
            if (!removed.Any())
            {
                return;
            }

            var remaining = Items.Except(removed).ToArray();
            Items.Clear();
            ((List<T>) Items).AddRange(remaining);

            OnReset();
        }

        public virtual void Reset(IEnumerable<T> range)
        {
            CheckReentrancy();

            Items.Clear();

            AddRange(range);
        }

        protected virtual void Dispatch(Action action)
        {
            var dispatcher = Platform.Shared?.Dispatcher;
            if (dispatcher?.CheckAccess() == false)
            {
                dispatcher.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void OnReset()
        {
            Dispatch(() =>
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }
    }
}