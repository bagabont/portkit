using System.Collections.ObjectModel;
using System.Windows.Input;
using PortKit.MVVM;

namespace PortKit.Bindings.UnitTests
{
    internal sealed class ItemViewModel : Bindable
    {
        private string _name;
        private string _description;
        private ICommand _command;
        private ObservableCollection<ItemViewModel> _subItems;
        private int _number;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public int Number
        {
            get => _number;
            set => Set(ref _number, value);
        }

        public ObservableCollection<ItemViewModel> SubItems
        {
            get => _subItems;
            set => Set(ref _subItems, value);
        }

        public ICommand Command
        {
            get => _command;
            set => Set(ref _command, value);
        }
    }
}