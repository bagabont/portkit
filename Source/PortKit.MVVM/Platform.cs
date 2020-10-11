using System;

namespace PortKit.MVVM
{
    public sealed class Platform
    {
        public sealed class PlatformBuilder
        {
            private IUIDispatcher _dispatcher;

            public PlatformBuilder WithDispatcher(IUIDispatcher dispatcher)
            {
                _dispatcher = dispatcher;

                return this;
            }

            public void Build()
            {
                _lazyShared = new Lazy<Platform>(() => new Platform
                {
                    Dispatcher = _dispatcher
                });
            }
        }

        private static Lazy<Platform> _lazyShared;

        public static Platform Shared => _lazyShared?.Value;

        public static PlatformBuilder Builder => new PlatformBuilder();

        public IUIDispatcher Dispatcher { get; private set; }

        private Platform()
        {
        }
    }
}