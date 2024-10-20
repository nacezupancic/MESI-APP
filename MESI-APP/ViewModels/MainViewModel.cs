using MESI_APP.Commands;
using MESI_APP.Http;
using MESI_APP.Models;
using MESI_APP.Models.SaveableCanvasModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
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
        private Stringcanvas _serverInboundUrlWrapper;
        public Stringcanvas ServerInboundUrlWrapper
        {
            get => _serverInboundUrlWrapper;
            set
            {
                _serverInboundUrlWrapper = value;
                OnPropertyChanged();
            }
        }
        private PortCanvas _serverInboundPortWrapper;
        public PortCanvas ServerInboundPortWrapper
        {
            get => _serverInboundPortWrapper;
            set
            {   _serverInboundPortWrapper = value;
                OnPropertyChanged();
            }
        }
        private Stringcanvas _clientOutboundUrlWrapper;
        public Stringcanvas ClientOutboundUrlWrapper
        {
            get => _clientOutboundUrlWrapper;
            set
            {
                _clientOutboundUrlWrapper = value;
                OnPropertyChanged();
            }
        }
        private PortCanvas _clientOutboundPortWrapper;
        public PortCanvas ClientOutboundPortWrapper
        {
            get => _clientOutboundPortWrapper;
            set
            {
                _clientOutboundPortWrapper = value;
                OnPropertyChanged();
            }
        }
        private Stringcanvas _messageBodyWrapper;
        public Stringcanvas MessageBodyWrapper
        {
            get => _messageBodyWrapper;
            set
            {
                _messageBodyWrapper = value;
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

        private Stringcanvas _receivedRequestsWrapper { get; set; }
        public Stringcanvas ReceivedRequestsWrapper
        {
            get => _receivedRequestsWrapper;
            set
            {
                _receivedRequestsWrapper = value;
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
        public void InitBindings() {
            foreach (var property in this.GetType().GetProperties().Where(x => typeof(CanvasPosition).IsAssignableFrom(x.PropertyType) && x.CanWrite))
            {
                var instance = Activator.CreateInstance(property.PropertyType);
                ((CanvasPosition)instance).BindableName = property.Name;
                property.SetValue(this, instance);
            }
        }
        public MainViewModel(ServerService server, ClientService clientService)
        {
            InitBindings();
            _httpServer = server;
            _httpServer.RequestReceived += OnRequestReceived;
            _clientService = clientService;

            ServerInboundUrlWrapper.TextValue = "http://localhost";
            SaveSettingsCommand = new RelayCommand(async execute => await SaveSettings());
            StartServerCommand = new RelayCommand(async execute => await StartServer());
            //StopServerCommand = new RelayCommand(execute => StopServer());
            StopServerCommand = new RelayCommand(async execute => await GetSettings());
            SendRequestCommand = new RelayCommand(async execute => await SendRequest());
            ReceivedRequests = new ObservableCollection<ReceivedRequestDTO>();
        }


        public async Task SendRequest() {
            await _clientService.SendPostRequest($"{ClientOutboundUrlWrapper.TextValue}:{ClientOutboundPortWrapper.Port}/", MessageBodyWrapper.TextValue);
        }

        public async Task GetSettings() {
            var fileContent = await File.ReadAllTextAsync("config.json");
            var jsonDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(fileContent);            
            foreach (var property in this.GetType().GetProperties().Where(x => typeof(CanvasPosition).IsAssignableFrom(x.PropertyType) && x.CanWrite))
            {
                if (jsonDict.ContainsKey(property.Name) && typeof(CanvasPosition).IsAssignableFrom(property.PropertyType))
                {
                    var jsonValue = jsonDict[property.Name];
                    var value = JsonSerializer.Deserialize(jsonValue.GetRawText(), property.PropertyType);
                    property.SetValue(this, value);
                }
            }
        }

        private async Task SaveSettings() {
            string file = "config.json";
            
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var property in this.GetType().GetProperties().Where(x => typeof(CanvasPosition).IsAssignableFrom(x.PropertyType)))
            {
                dict[property.Name] = property.GetValue(this);
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(dict,options); ;
            File.WriteAllText(file, jsonString);
        }

        private async Task StartServer()
        {
            _httpServer.ConfigServer(ServerInboundUrlWrapper.TextValue, ServerInboundPortWrapper.Port.Value);
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

        public async Task ElementDragged() {
            if (AutoSave)
            {
                await SaveSettings();
            }
        }

    }
}
