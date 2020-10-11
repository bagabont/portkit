namespace PortKit.Navigation
{
    public interface IView<TViewModel> : IView
    {
        new TViewModel DataContext { get; set; }
    }
}