using System;
using System.Windows.Input;
using FluentAssertions;
using NUnit.Framework;

namespace PortKit.MVVM.UnitTests
{
    [TestFixture]
    internal sealed class RelayCommandTests : Bindable
    {
        private string _name;

        private string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        [Test]
        public void Watch_InstancePropertyChanged_RaisesCanExecuteChanged()
        {
            var command = new RelayCommand(() => { });
            command.Watch(() => Name);

            using var monitor = command.Monitor();

            Name = Guid.NewGuid().ToString();
            monitor.Should().Raise(nameof(ICommand.CanExecuteChanged));
        }

        [Test]
        public void Unwatch_InstancePropertyChanged_DoesNotRaiseCanExecuteChanged()
        {
            var command = new RelayCommand(() => { });
            command.Watch(() => Name);

            using var monitor = command.Monitor();

            command.Unwatch(() => Name);
            Name = Guid.NewGuid().ToString();

            monitor.Should().NotRaise(nameof(ICommand.CanExecuteChanged));
        }
    }
}