using PortKit.MVVM;

namespace PortKit.Bindings.UnitTests.Data
{
    internal class SuperClass : Bindable
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
    }
}