using MESI_APP.Helpers;
using MESI_APP.Models;
using MESI_APP.Services;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace MESI_APP.Http
{
    public class ServerService : IServerService
    {
        private readonly ILoggerService _logger;
        private HttpListener _httpListener;
        public event Action<ReceivedRequestDTO> RequestReceived;
        private const string DefaultResponseMessageOK = "Hello, MESI-APP user!";
        private const string DefaultResponseMessageBadRequest = "Invalid request content";
        
        public ServerService(ILoggerService loggerService) {
            InitListener();
            _logger = loggerService;
        }
        public async Task Start()
        {
            try
            {
                // Start HTTP server on set url:port
                _logger.Info($"Starting HTTP server: {_httpListener.Prefixes.FirstOrDefault()}");
                if (_httpListener.IsListening)
                {
                    _logger.Info("Server is already running.");
                    return;
                }
                _httpListener.Start();

                while (_httpListener.IsListening)
                {
                    var context = await _httpListener.GetContextAsync();
                    await HandleRequest(context);
                }

            }
            catch (HttpListenerException ex)
            {
                _logger.Error($"Http server was stopped: {ex.Message}");
                InitListener();
            }
            catch (Exception ex)
            {
                _logger.Error($"Error starting HTTP server: {ex.Message}");
                Debug.WriteLine($"{ex.Message}");
                InitListener();
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            DateTime dt = DateTime.Now;
            var response = context.Response;
            var request = context.Request;
            var headers = request.Headers;
            bool isJson = true;
            try
            {
                string content = string.Empty;
                if (request.HasEntityBody)
                {
                    using (var stream = new StreamReader(request.InputStream))
                    {
                        content = await stream.ReadToEndAsync();
                    }
                    isJson = await SerializationHelper.IsJsonFormat(content);
                }

                // Prepare and send received request to listeners (ViewModel)
                var headerValues = request.Headers.AllKeys.Select(x => $"{x} | {request.Headers[x]}");
                RequestReceived?.Invoke(new ReceivedRequestDTO(dt, content, string.Join('\n', headerValues), request.HttpMethod));

                // Prepare response
                string responseString = isJson ? DefaultResponseMessageOK : DefaultResponseMessageBadRequest;
                response.ContentType = "text/plain";
                response.StatusCode = isJson ? 200 : 400;
                using (var writer = new StreamWriter(response.OutputStream))
                {
                    await writer.WriteAsync(responseString);
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error handling request: {ex.Message}");
            }
            finally {
                response.Close();
            }
        }

        public bool ConfigServer(string url, int port) {
            _logger.Info("Configuring server...");
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) || port < 1 || port > 65535)
            {
                _logger.Error("Server configuration failed, URL:Port is invalid");
                return false;
            }
            // Remove ending slashes from url
            url = Regex.Replace(url, "/+$", "");

            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add($"{url}:{port}/");
            return true;
        }

        public void Stop()
        {
            if (!_httpListener.IsListening) {
                _logger.Info("Server is not running.");
                return;
            }
            else
            {
                _httpListener.Stop();
            }
            _httpListener.Close();
            _logger.Info("Server was stopped.");
            // Reinitialize httpListener becasue the current one is disposed
            InitListener();        
        }

        private void InitListener() {
            _httpListener = new HttpListener();
        }
    }
}
