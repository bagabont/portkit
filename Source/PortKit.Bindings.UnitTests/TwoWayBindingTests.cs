using FluentAssertions;
using NUnit.Framework;

namespace PortKit.Bindings.UnitTests
{
    [TestFixture]
    internal sealed class TwoWayBindingTests
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
            var binding = this.Set(
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

            var binding = this.Set(
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