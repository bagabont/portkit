using System;

namespace PortKit.Navigation
{
    public interface IViewFactory
    {
        IPage Create(Type page);
    }
}