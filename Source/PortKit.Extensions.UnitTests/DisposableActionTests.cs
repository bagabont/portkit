using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace PortKit.Extensions.UnitTests
{
    [TestFixture]
    internal sealed class DisposableActionTests
    {
        [Test]
        public void SingleCall_Dispose_ActionIsInvoked()
        {
            const int expected = 1;
            var count = 0;

            void UpdateCallCount()
            {
                count++;
            }

            var disposable = new DisposableAction(UpdateCallCount);
            disposable.Dispose();

            count.Should().Be(expected);
        }

        [Test]
        public void MultipleCall_Dispose_ActionIsInvokedOnce()
        {
            const int expected = 1;
            var count = 0;

            void UpdateCallCount()
            {
                count++;
            }

            var disposable = new DisposableAction(UpdateCallCount);

            disposable.Dispose();
            disposable.Dispose();
            disposable.Dispose();

            count.Should().Be(expected);
        }

        [Test]
        public async Task MultipleCall_MultiThreadedDispose_ActionIsInvokedOnce()
        {
            const int expected = 1;
            var count = 0;

            void UpdateCallCount()
            {
                count++;
            }

            var disposable = new DisposableAction(UpdateCallCount);

            await Task.WhenAll(
                Enumerable.Range(0, 999)
                    .Select(x => Task.Run(() => disposable.Dispose()))
            );

            count.Should().Be(expected);
        }
    }
}