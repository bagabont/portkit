using System;

namespace PortKit.Navigation
{
    public interface IView
    {
        event EventHandler Dismissed;

        object DataContext { get; set; }
    }
}