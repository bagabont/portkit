using FluentAssertions;
using NUnit.Framework;
using PortKit.Bindings.UnitTests.Data;

namespace PortKit.Bindings.UnitTests
{
    [TestFixture]
    internal sealed class BindingTwoWayTests
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
        public void TwoWayBinding_SettingBinding_UpdatesTargetProperty()
        {
            const string expected = "test";

            _sourceItem.Name = expected;
            var binding = this.SetBinding(
                () => _sourceItem.Name,
                () => _targetItem.Name,
                BindingMode.TwoWay
            );

            using (binding)
            {
                _targetItem.Name.Should().Be(expected);
            }
        }

        [Test]
        public void TwoWayBinding_TargetPropertyChanged_UpdatesSourceProperty()
        {
            const string expected = "testName";

            var binding = this.SetBinding(
                () => _sourceItem.Name,
                () => _targetItem.Name,
                BindingMode.TwoWay
            );

            using (binding)
            {
                _targetItem.Name = expected;

                _targetItem.Name.Should().Be(expected);
            }
        }
    }
}