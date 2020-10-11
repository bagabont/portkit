using FluentAssertions;
using NUnit.Framework;

namespace PortKit.Bindings.UnitTests
{
    [TestFixture]
    internal sealed class OneTimeBindingTests
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
        public void OneTimeBinding_SettingBinding_UpdatesTargetProperty()
        {
            const string expected = "test";
            _sourceItem.Name = expected;

            var binding = this.Set(
                () => _sourceItem.Name,
                () => _targetItem.Name,
                BindingMode.OneTime
            );

            using (binding)
            {
                _targetItem.Name.Should().Be(expected);
            }
        }

        [Test]
        public void OneTimeBinding_SourceUpdated_DoesNotUpdateTarget()
        {
            const string expected = "test";
            _sourceItem.Name = expected;

            var binding = this.Set(
                () => _sourceItem.Name,
                () => _targetItem.Name,
                BindingMode.OneTime
            );

            using (binding)
            {
                _sourceItem.Name = "test2";
                _targetItem.Name.Should().Be(expected);
            }
        }

        [Test]
        public void OneTimeBinding_TargetUpdated_DoesNotUpdateSource()
        {
            const string expected = "test";
            _sourceItem.Name = expected;

            var binding = this.Set(
                () => _sourceItem.Name,
                () => _targetItem.Name,
                BindingMode.OneTime
            );

            using (binding)
            {
                _targetItem.Name = "test2";
                _sourceItem.Name.Should().Be(expected);
            }
        }
    }
}