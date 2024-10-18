using MESI_APP.Commands;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;

namespace MESI_APP.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public ICommand SaveSettingsCommand{ get; set; }
        private string _serverOutboundUrl;
        public string ServerOutboundUrl
        {
            get => _serverOutboundUrl;
            set
            {
                _serverOutboundUrl = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel()
        {
            ServerOutboundUrl = "Binding works";
            SaveSettingsCommand = new RelayCommand(execute => SaveSettings());
        }

        private void SaveSettings() {
            ServerOutboundUrl += "Yay";
        }
    }
}
