using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace PortKit.MVVM.UnitTests
{
    [TestFixture]
    public sealed class BindableCollectionTests
    {
        [Test]
        public void BindableCollection_AddRange_ItemsAreAdded()
        {
            var expected = new List<object> { 1, 2, null, new object() };
            var collection = new BindableCollection<object>();

            collection.AddRange(expected);

            collection.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void BindableCollection_AddRange_RaisesResetCollectionChanged()
        {
            var range = new List<object> { 1, 2, 3 };
            var collection = new BindableCollection<object>();

            using var monitor = collection.Monitor();
            collection.ExecutionTimeOf(x => x.AddRange(range))
                .Should()
                .BeLessThan(TimeSpan.FromMilliseconds(50));

            monitor.Should()
                .Raise(nameof(INotifyCollectionChanged.CollectionChanged))
                .WithArgs<NotifyCollectionChangedEventArgs>(x => x.Action == NotifyCollectionChangedAction.Reset);
        }

        [Test]
        public void BindableCollection_RemoveRange_RaisesResetCollectionChanged()
        {
            var collection = new BindableCollection<int> { 1, 2, 3, 4, 5 };
            var range = collection.Skip(3).ToArray();

            using var monitor = collection.Monitor();
            collection.RemoveRange(range);

            monitor.Should()
                .Raise(nameof(INotifyCollectionChanged.CollectionChanged))
                .WithArgs<NotifyCollectionChangedEventArgs>(x =>
                    x.Action == NotifyCollectionChangedAction.Reset && x.OldItems == null);
        }

        [Test]
        public void BindableCollection_RemoveRange_ItemsAreRemoved()
        {
            var expected = new List<int> { 1, 2, 3 };
            var collection = new BindableCollection<int> { 1, 2, 3, 4, 5 };
            var range = collection.Skip(3).ToArray();

            collection.RemoveRange(range);

            collection.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void BindableCollection_ResetWithItems_CollectionIsResetToNewItems()
        {
            var expected = new List<object> { 3, 2, 1 };
            var collection = new BindableCollection<object> { 1, 2, 3 };

            using (var monitor = collection.Monitor())
            {
                collection.Reset(expected);

                monitor.Should()
                    .Raise(nameof(INotifyCollectionChanged.CollectionChanged))
                    .WithArgs<NotifyCollectionChangedEventArgs>(x => x.Action == NotifyCollectionChangedAction.Reset);
            }

            collection.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void BindableCollection_Execute_ItemsAreUpdated()
        {
            var expected = new List<int> { 2, 3, 4 };
            var collection = new BindableCollection<int> { 1, 2, 3 };

            using (var monitor = collection.Monitor())
            {
                collection.Execute(items =>
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        items[i] += 1;
                    }
                });

                monitor.Should()
                    .Raise(nameof(INotifyCollectionChanged.CollectionChanged))
                    .WithArgs<NotifyCollectionChangedEventArgs>(x => x.Action == NotifyCollectionChangedAction.Reset);

                collection.Should().BeEquivalentTo(expected);
            }
        }

        [Test]
        public void BindableCollection_ModificationDuringCollectionChangedEvent_ThrowsInvalidOperationException()
        {
            var collection = new BindableCollection<int>();

            void Handler1(object sender, NotifyCollectionChangedEventArgs e)
            {
            }

            void Handler2(object sender, NotifyCollectionChangedEventArgs e)
            {
                collection.Invoking(x => x.AddRange(Enumerable.Range(1, 10)))
                    .Should()
                    .Throw<InvalidOperationException>();
            }

            collection.CollectionChanged += Handler1;
            collection.CollectionChanged += Handler2;

            collection.AddRange(Enumerable.Range(1, 10));
        }

        [Test]
        public async Task BindableCollection_ExecuteFromMultipleThreads_AvoidRaceConditions()
        {
            var collection = new BindableCollection<int>();
            var tasks = new List<Task>();
            const int itemsCount = 1000;

            for (var i = 0; i < itemsCount; i++)
            {
                var idx = i;
                tasks.Add(Task.Run(() => collection.Execute(items => items.Add(idx))));
            }

            await Task.WhenAll(tasks);

            collection.Count.Should().Be(itemsCount);
        }

        [Test]
        public void BindableCollection_LargeRangeRemoved_PerformanceIsNotAffected()
        {
            var collection = new BindableCollection<int>(Enumerable.Range(0, 600_000));
            var range = Enumerable.Range(200_000, 300_000).Reverse();

            collection.ExecutionTimeOf(x => x.RemoveRange(range))
                .Should()
                .BeLessThan(TimeSpan.FromMilliseconds(500));

            collection.Count.Should().Be(300_000);
        }
    }
}