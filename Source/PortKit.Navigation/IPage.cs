using System;

namespace PortKit.Navigation
{
    public interface IPage : IDataView
    {
        event EventHandler Closing;

        bool IsClosing { get; set; }
    }
}