using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MESI_APP.Http;
using MESI_APP.Models;
using MESI_APP.Models.SaveableCanvasModels;
using MESI_APP.Services;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text.Json;
using System.Windows;


namespace MESI_APP.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IServerService _httpServer;
        private readonly IClientService _clientService;
        private readonly ISettingsService _settingsService;
        private readonly ILoggerService _loggerService;
        private List<PropertyInfo> _canvasPropertyInfo;

        #region Binding properties
        [ObservableProperty]
        private WindowSize _windowSize;
        [ObservableProperty]
        private Stringcanvas _serverInboundUrlWrapper;

        [ObservableProperty]
        private PortCanvas _serverInboundPortWrapper;

        [ObservableProperty]
        private Stringcanvas _clientOutboundUrlWrapper;

        [ObservableProperty]
        private PortCanvas _clientOutboundPortWrapper;

        [ObservableProperty]
        private Stringcanvas _messageBodyWrapper;

        [ObservableProperty]
        private bool _autoSave = true; // By default, always autosave settings/positions
        
        [ObservableProperty]
        private Stringcanvas _receivedRequestsWrapper;
        
        [ObservableProperty]
        private ObservableCollection<ReceivedRequestDTO> _receivedRequests;
        
        [ObservableProperty]
        private ObservableCollection<LoggerMsg> _logList;
        
        [ObservableProperty]
        private Stringcanvas _loggerWrapper;
        [ObservableProperty]
        private RequestHeaderCanvas _headersCanvas;
        #endregion        

        public MainViewModel(IServerService server, IClientService clientService, ISettingsService settingsService, ILoggerService loggerService)
        {
            InitBindings();

            _loggerService = loggerService;
            _httpServer = server;
            _clientService = clientService;
            _settingsService = settingsService;

            _loggerService.OnLog += HandleLog;
            _httpServer.RequestReceived += OnRequestReceived;
        }
        public void InitBindings()
        {
            _canvasPropertyInfo = new List<PropertyInfo>();
            LogList = new ObservableCollection<LoggerMsg>();
            ReceivedRequests = new ObservableCollection<ReceivedRequestDTO>();
            foreach (var property in this.GetType().GetProperties().Where(x => typeof(CanvasPosition).IsAssignableFrom(x.PropertyType) && x.CanWrite))
            {
                var instance = Activator.CreateInstance(property.PropertyType);
                ((CanvasPosition)instance).BindableName = property.Name;
                property.SetValue(this, instance);
                _canvasPropertyInfo.Add(property);
            }
        }
        public async Task LoadConfiguration(bool init = false) {
            var jsonDict = await _settingsService.GetSettings(init);
            if (jsonDict != null)
            {
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
        }

        [RelayCommand]
        private async Task SendRequest() {
            if (!ValidateClientConfig())
            {
                return;
            }
            var headers = HeadersCanvas.HeaderList.Where(h => !string.IsNullOrEmpty(h.HeaderKey) && !string.IsNullOrEmpty(h.HeaderValue));
            await _clientService.SendPostRequest($"{ClientOutboundUrlWrapper.TextValue}:{ClientOutboundPortWrapper.Port}/", headers, MessageBodyWrapper.TextValue);
        }

        [RelayCommand]
        private async Task SaveSettings() {
            try
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                foreach (var property in _canvasPropertyInfo)
                {
                    dict[property.Name] = property.GetValue(this);
                }
                await _settingsService.SaveSettings(dict);
            }
            catch (Exception ex) {
                _loggerService.Error($"Save settings failed. {ex.Message}");
            }
        }
        [RelayCommand]
        private async Task ResetSettings()
        {
            try
            {
                await LoadConfiguration(true);
                await SaveSettings();
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Reset settings failed. {ex.Message}");
            }
        }
        [RelayCommand]
        private async Task StartServer()
        {
            if (!ValidateServerConfig()) {
                return;
            }
            if (_httpServer.ConfigServer(ServerInboundUrlWrapper.TextValue, ServerInboundPortWrapper.Port.Value))
            {
                await _httpServer.Start();
            }
        }
        [RelayCommand]
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

        private bool ValidateServerConfig() => ValidateConfig(ServerInboundPortWrapper, ServerInboundUrlWrapper, "Server");
        private bool ValidateClientConfig() => ValidateConfig(ClientOutboundPortWrapper, ClientOutboundUrlWrapper, "Client");
        private bool ValidateConfig(PortCanvas portWrapper, Stringcanvas urlWrapper, string entityName)
        {
            if (portWrapper == null || !portWrapper.Port.HasValue)
            {
                _loggerService.Error($"{entityName} port is invalid");
                return false;
            }

            if (urlWrapper == null || string.IsNullOrEmpty(urlWrapper.TextValue) || !Uri.IsWellFormedUriString(urlWrapper.TextValue, UriKind.Absolute))
            {
                _loggerService.Error($"{entityName} url is invalid");
                return false;
            }

            return true;
        }
    }
}
