using MESI_APP.Commands;
using MESI_APP.Http;
using MESI_APP.Models;
using MESI_APP.Models.Enums;
using MESI_APP.Models.SaveableCanvasModels;
using MESI_APP.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;

namespace MESI_APP.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ServerService _httpServer;
        private readonly ClientService _clientService;
        private readonly SettingsService _settingsService;
        private readonly LoggerService _loggerService;
        private List<PropertyInfo> _canvasPropertyInfo;

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
        private ObservableCollection<LoggerMsg> _logList;
        public ObservableCollection<LoggerMsg> LogList
        {
            get => _logList;
            set
            {
                _logList = value;
                OnPropertyChanged();
            }
        }
        private Stringcanvas _loggerWrapper { get; set; }
        public Stringcanvas LoggerWrapper
        {
            get => _loggerWrapper;
            set
            {
                _loggerWrapper = value;
                OnPropertyChanged();
            }
        }
        #endregion        
        public MainViewModel(ServerService server, ClientService clientService, SettingsService settingsService, LoggerService loggerService)
        {
            InitBindings();
            _loggerService = loggerService;
            _loggerService.OnLog += HandleLog;
            _httpServer = server;
            _httpServer.RequestReceived += OnRequestReceived;
            _clientService = clientService;
            _settingsService = settingsService;

            ServerInboundUrlWrapper.TextValue = "http://localhost";
            SaveSettingsCommand = new RelayCommand(async execute => await SaveSettings());
            StartServerCommand = new RelayCommand(async execute => await StartServer());
            //StopServerCommand = new RelayCommand(execute => StopServer());
            StopServerCommand = new RelayCommand(async execute => await LoadConfiguration());
            SendRequestCommand = new RelayCommand(async execute => await SendRequest());
            ReceivedRequests = new ObservableCollection<ReceivedRequestDTO>();
            LogList = new ObservableCollection<LoggerMsg>();
        }
        public void InitBindings()
        {
            _canvasPropertyInfo = new List<PropertyInfo>();
            foreach (var property in this.GetType().GetProperties().Where(x => typeof(CanvasPosition).IsAssignableFrom(x.PropertyType) && x.CanWrite))
            {
                var instance = Activator.CreateInstance(property.PropertyType);
                ((CanvasPosition)instance).BindableName = property.Name;
                property.SetValue(this, instance);
                _canvasPropertyInfo.Add(property);
            }
        }

        public async Task SendRequest() {
            await _clientService.SendPostRequest($"{ClientOutboundUrlWrapper.TextValue}:{ClientOutboundPortWrapper.Port}/", MessageBodyWrapper.TextValue);
        }

        public async Task LoadConfiguration() {
            var jsonDict = await _settingsService.GetSettings();           
            foreach (var property in _canvasPropertyInfo)
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
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (var property in _canvasPropertyInfo)
                //foreach (var property in this.GetType().GetProperties().Where(x => typeof(CanvasPosition).IsAssignableFrom(x.PropertyType)))
            {
                dict[property.Name] = property.GetValue(this);
            }
            await _settingsService.SaveSettings(dict);            
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
        private void HandleLog(LoggerMsg log)
        {
            // UI can be only updated from main thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                LogList.Insert(0,log);
            });
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
