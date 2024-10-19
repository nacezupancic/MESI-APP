using MESI_APP.Commands;
using MESI_APP.Http;
using MESI_APP.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace MESI_APP.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private Server _httpServer;


        public ICommand SaveSettingsCommand { get; set; }
        public ICommand StartServerCommand { get; set; }
        public ICommand StopServerCommand { get; set; }
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
        private int _serverOutboundPort;
        public int ServerOutboundPort
        {
            get => _serverOutboundPort;
            set
            {
                if (value > 0 && value < 65537) {
                    _serverOutboundPort = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _httpResponseMsg;
        public string HttpResponseMsg
        {
            get => _httpResponseMsg;
            set
            {
                _httpResponseMsg = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ReceivedRequestDTO> _receivedRequests;
        public ObservableCollection<ReceivedRequestDTO> ReceivedRequests
        {
            get => _receivedRequests;
            set
            {
                _receivedRequests = value;
                OnPropertyChanged();
            }
        }
        public MainViewModel(Server server)
        {
            _httpServer = server;
            _httpServer.RequestReceived += OnRequestReceived;

            ServerOutboundUrl = "http://localhost";
            SaveSettingsCommand = new RelayCommand(async execute => await SaveSettings());
            StartServerCommand = new RelayCommand(async execute => await StartServer());
            StopServerCommand = new RelayCommand(execute => StopServer());
            ReceivedRequests = new ObservableCollection<ReceivedRequestDTO>();
        }

        private async Task SaveSettings() {
            ServerOutboundUrl += "Yay";
        }

        private async Task StartServer()
        {
            _httpServer.ConfigServer(ServerOutboundUrl, ServerOutboundPort);
            await _httpServer.Start();
        }
        private void StopServer()
        {
            _httpServer.Stop();
        }

        private void OnRequestReceived(ReceivedRequestDTO dto)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                // We are on the UI thread, safe to update the collection
                ReceivedRequests.Add(dto);
            }
            else
            {
                // We're on a background thread, invoke on the UI thread
                Application.Current.Dispatcher.Invoke(() => OnRequestReceived(dto));
            }
        }

    }
}
