using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;
using PortKit.Bindings.UnitTests.Data;
using PortKit.MVVM;

namespace PortKit.Bindings.UnitTests
{
    [TestFixture]
    internal sealed class BindingOneWayTests
    {
        private ItemViewModel _sourceItem;
        private ItemViewModel _targetItem;

        [SetUp]
        public void SetUp()
        {
            _sourceItem = new ItemViewModel();
            _targetItem = new ItemViewModel();
        }

        [Test]
        public void OneWayBinding_SettingBinding_UpdatesTargetProperty()
        {
            const string expected = "test";

            _sourceItem.Name = expected;
            var binding = this.Set(
                () => _sourceItem.Name,
                () => _targetItem.Name,
                BindingMode.OneWay
            );

            using (binding)
            {
                _targetItem.Name.Should().Be(expected);
            }
        }

        [Test]
        public void OneWayBinding_SourcePropertyChanged_UpdatesTargetProperty()
        {
            var binding = this.Set(
                () => _sourceItem.Name,
                () => _targetItem.Name,
                BindingMode.OneWay
            );

            using (binding)
            {
                const string expected = "test";
                _sourceItem.Name = expected;

                _targetItem.Name.Should().Be(expected);
            }
        }

        [Test]
        public void OneWayBinding_DifferentPropertyTypesWithConverter_UpdatesTargetProperty()
        {
            const string expectedDescription = "Items count=1";
            var binding = this.Set(
                () => _sourceItem.SubItems,
                () => _targetItem.Description,
                BindingMode.OneWay,
                items => "Items count=" + items?.Count
            );

            using (binding)
            {
                _sourceItem.SubItems = new ObservableCollection<ItemViewModel>();
                _sourceItem.SubItems.Add(new ItemViewModel());

                _targetItem.Description.Should().Be(expectedDescription);
            }
        }

        [Test]
        public void OneWayBinding_TargetPropertyUpdated_DoesNotUpdateSource()
        {
            var binding = this.Set(
                () => _sourceItem.Name,
                () => _targetItem.Name,
                BindingMode.OneWay
            );

            using (binding)
            {
                const string expected = "source";
                _sourceItem.Name = expected;
                _targetItem.Name.Should().Be(expected);
                _targetItem.Name = "test";

                _sourceItem.Name.Should().Be(expected);
            }
        }

        [Test]
        public void OneWayBinding_PropertyInstanceInExpressionChanges_UpdatesTargetProperty()
        {
            _sourceItem.Command = new RelayCommand<ItemViewModel>(item =>
            {
                _sourceItem.SubItems ??= new ObservableCollection<ItemViewModel>();
                _sourceItem.SubItems.Add(item);
            });

            var binding = this.Set(
                () => _sourceItem.SubItems.Count,
                () => _targetItem.Description,
                BindingMode.OneWay
            );

            _sourceItem.Command.Execute(new ItemViewModel());

            using (binding)
            {
                _targetItem.Description.Should().Be(_sourceItem.SubItems.Count.ToString());
            }
        }

        [Test]
        public void OneWayBinding_FallbackWithDefaultConverter_TargetPropertySetToFallback()
        {
            const int expected = 100;
            var binding = this.Set(
                    () => _sourceItem.Name.Length,
                    () => _targetItem.Name,
                    BindingMode.OneWay)
                .WithFallback(expected);

            using (binding)
            {
                _targetItem.Name.Should().Be(expected.ToString());
            }
        }
    }
}