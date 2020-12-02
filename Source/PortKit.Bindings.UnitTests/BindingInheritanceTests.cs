using FluentAssertions;
using NUnit.Framework;
using PortKit.Bindings.UnitTests.Data;

namespace PortKit.Bindings.UnitTests
{
    [TestFixture]
    internal class BindingInheritanceTests
    {
        private SubClass _sourceItem;
        private SuperClass _targetItem;

        [SetUp]
        public void SetUp()
        {
            _sourceItem = new SubClass();
            _targetItem = new SuperClass();
        }

        [Test]
        public void OneWayBinding_SettingBinding_UpdatesTargetProperty()
        {
            const string expected = "test";

            _sourceItem.Name = expected;
            var binding = this.SetBinding(
                () => _sourceItem.Name,
                () => _targetItem.Name
            );

            using (binding)
            {
                _targetItem.Name.Should().Be(expected);
            }
        }

        [Test]
        public void OneWayBinding_SourcePropertyChanged_UpdatesTargetProperty()
        {
            var binding = this.SetBinding(
                () => _sourceItem.Name,
                () => _targetItem.Name
            );

            using (binding)
            {
                const string expected = "test";
                _sourceItem.Name = expected;

                _targetItem.Name.Should().Be(expected);
            }
        }

        [Test]
        public void OneWayBinding_TargetPropertyUpdated_DoesNotUpdateSource()
        {
            var binding = this.SetBinding(
                () => _sourceItem.Name,
                () => _targetItem.Name
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
        public void OneWayBinding_FallbackWithDefaultConverter_TargetPropertySetToFallback()
        {
            const int expected = 100;
            var binding = this.SetBinding(
                    () => _sourceItem.Name.Length,
                    () => _targetItem.Name)
                .WithFallback(expected);

            using (binding)
            {
                _targetItem.Name.Should().Be(expected.ToString());
            }
        }
    }
}