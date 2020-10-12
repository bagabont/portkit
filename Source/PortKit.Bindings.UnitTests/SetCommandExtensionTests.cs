using System;
using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;
using PortKit.Bindings.Extensions;
using PortKit.Bindings.UnitTests.Data;
using PortKit.MVVM;

namespace PortKit.Bindings.UnitTests
{
    [TestFixture]
    internal sealed class SetCommandExtensionTests
    {
        [Test]
        public void SetCommandHandlers_NoArgsEventRaisedWithEnabledCommand_ExecutesCommand()
        {
            var view = new TestView();
            var hasCommandExecuted = false;
            var command = new RelayCommand(() => hasCommandExecuted = true);
            using (view.SetCommand(command, h => view.NoArgumentsEvent += h, h => view.NoArgumentsEvent -= h))
            {
                view.RaiseNoArgumentsEvent();

                hasCommandExecuted.Should().BeTrue();
            }
        }

        [Test]
        public void SetCommandHandlers_ArgsEventRaisedWithEnabledCommand_ExecutesCommand()
        {
            var view = new TestView();
            bool? commandExecuteParameter = null;
            var command = new RelayCommand<bool?>(value => commandExecuteParameter = value);
            using (view.SetCommand<StubEventArgs>(
                command,
                h => view.ArgumentsEvent += h,
                h => view.ArgumentsEvent -= h,
                e => e.Args.Data))
            {
                view.RaiseArgumentsEvent(true);

                commandExecuteParameter.Should().BeTrue();
            }
        }

        [Test]
        public void SetCommandHandlers_PropertyChangedEventRaisedWithEnabledCommand_ExecutesCommand()
        {
            const string expectedParameter = "testProp";
            var view = new TestView();
            string actualParameter = null;
            var command = new RelayCommand<string>(value => actualParameter = value);
            using (view.SetCommand<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                command,
                h => view.PropertyChangedEvent += h,
                h => view.PropertyChangedEvent -= h,
                e => e.Args.PropertyName))
            {
                view.RaisePropertyChanged(expectedParameter);

                actualParameter.Should().Be(expectedParameter);
            }
        }

        [Test]
        public void SetCommandEventName_NoArgsEventRaisedWithEnabledCommand_ExecutesCommand()
        {
            var view = new TestView();
            var hasCommandExecuted = false;
            var command = new RelayCommand(() => hasCommandExecuted = true);
            using (view.SetCommand(nameof(view.NoArgumentsEvent), command))
            {
                view.RaiseNoArgumentsEvent();

                hasCommandExecuted.Should().BeTrue();
            }
        }

        [Test]
        public void SetCommandEventName_ArgsEventRaisedWithEnabledCommand_ExecutesCommand()
        {
            var view = new TestView();
            bool? commandExecuteParameter = null;
            var command = new RelayCommand<bool?>(value => commandExecuteParameter = value);
            using (view.SetCommand<StubEventArgs>(nameof(view.ArgumentsEvent), command, e => e.Args.Data))
            {
                view.RaiseArgumentsEvent(true);

                commandExecuteParameter.Should().BeTrue();
            }
        }

        [Test]
        public void SetCommandEventName_PropertyChangedEventRaisedWithEnabledCommand_ExecutesCommand()
        {
            const string expectedParameter = "testProp";
            var view = new TestView();
            string actualParameter = null;
            var command = new RelayCommand<string>(value => actualParameter = value);
            using (view.SetCommand<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                nameof(view.PropertyChangedEvent),
                command,
                e => e.Args.PropertyName))
            {
                view.RaisePropertyChanged(expectedParameter);

                actualParameter.Should().Be(expectedParameter);
            }
        }

        [Test]
        public void SetCommandEventName_InternalEvent_ThrowsException()
        {
            var view = new TestView();
            var command = new RelayCommand(() => { });
            Action setCommandAction = () => view.SetCommand(nameof(view.InternalEvent), command);
            setCommandAction.Should().Throw<ArgumentException>();
        }
    }
}