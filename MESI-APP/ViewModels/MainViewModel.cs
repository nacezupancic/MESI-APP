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
        private ServerService _httpServer;
        private ClientService _clientService;

        #region Binding properties
        public ICommand SaveSettingsCommand { get; set; }
        public ICommand StartServerCommand { get; set; }
        public ICommand StopServerCommand { get; set; }
        public ICommand SendRequestCommand { get; set; }
        private string _serverInboundUrl;
        public string ServerInboundUrl
        {
            get => _serverInboundUrl;
            set
            {
                _serverInboundUrl = value;
                OnPropertyChanged();
            }
        }
        private int _serverInboundPort;
        public int ServerInboundPort
        {
            get => _serverInboundPort;
            set
            {
                if (value > 0 && value < 65537) {
                    _serverInboundPort = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _clientOutboundUrl;
        public string ClientOutboundUrl
        {
            get => _clientOutboundUrl;
            set
            {
                _clientOutboundUrl = value;
                OnPropertyChanged();
            }
        }
        private int _clientOutboundPort;
        public int ClientOutboundPort
        {
            get => _clientOutboundPort;
            set
            {
                if (value > 0 && value < 65537)
                {
                    _clientOutboundPort = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _messageBody;
        public string MessageBody
        {
            get => _messageBody;
            set
            {
                _messageBody = value;
                OnPropertyChanged();
            }
        }
        private bool _autoSave;
        public bool AutoSave
        {
            get => _autoSave;
            set
            {
                _autoSave = value;
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
        #endregion
        public MainViewModel(ServerService server, ClientService clientService)
        {
            _httpServer = server;
            _httpServer.RequestReceived += OnRequestReceived;
            _clientService = clientService;

            ServerInboundUrl = "http://localhost";
            SaveSettingsCommand = new RelayCommand(async execute => await SaveSettings());
            StartServerCommand = new RelayCommand(async execute => await StartServer());
            StopServerCommand = new RelayCommand(execute => StopServer());
            SendRequestCommand = new RelayCommand(async execute => await SendRequest());
            ReceivedRequests = new ObservableCollection<ReceivedRequestDTO>();
        }

        public async Task SendRequest() {
            await _clientService.SendPostRequest($"{ClientOutboundUrl}:{ClientOutboundPort}/", MessageBody);
        }

        private async Task SaveSettings() {
            ServerInboundUrl += "Yay";
        }

        private async Task StartServer()
        {
            _httpServer.ConfigServer(ServerInboundUrl, ServerInboundPort);
            await _httpServer.Start();
        }
        private void StopServer()
        {
            _httpServer.Stop();
        }

        private void OnRequestReceived(ReceivedRequestDTO dto)
        {
            // UI can be only updated from main thread
            if (Application.Current.Dispatcher.CheckAccess())
            {
                ReceivedRequests.Insert(0,dto);
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => OnRequestReceived(dto));
            }
        }

    }
}
