using MESI_APP.Models;
using MESI_APP.Services;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace MESI_APP.Http
{
    public class ServerService 
    {
        private readonly LoggerService _logger;
        private HttpListener _httpListener;
        public event Action<ReceivedRequestDTO> RequestReceived;
        private bool _isRunning;
        public ServerService(LoggerService loggerService) {
            InitListener();
            _logger = loggerService;
        }
        public async Task Start()
        {
            try
            {
                // Start HTTP server on set url:port
                _logger.Info($"Starting HTTP server: {_httpListener.Prefixes.FirstOrDefault()}");
                if (_isRunning)
                {
                    _logger.Info("Server is already running.");
                    return;
                }
                _isRunning = true;
                _httpListener.Start();

                while (_isRunning)
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
            try
            {
                DateTime dt = DateTime.Now;
                var response = context.Response;
                var request = context.Request;
                var headers = request.Headers;

                string content = "";
                using (var stream = new StreamReader(request.InputStream))
                {
                    content = await stream.ReadToEndAsync();
                }
                // Send received request to listeners (ViewModel)
                var headerValues = request.Headers.AllKeys.Select(x => $"{x} | {request.Headers[x]}");
                RequestReceived?.Invoke(new ReceivedRequestDTO(dt, content, string.Join('\n', headerValues), request.HttpMethod));

                // Prepare response
                string responseString = "Hello, MESI-APP user!";
                response.ContentType = "text/plain";

                using (var writer = new StreamWriter(response.OutputStream))
                {
                    await writer.WriteAsync(responseString);
                }
                response.Close();
            }
            catch (Exception ex) {
                _logger.Error($"Error handling request: {ex.Message}");
            }
        }

        public bool ConfigServer(string url, int port) {
            if (!Uri.IsWellFormedUriString(url, UriKind.Absolute) || port < 1 || port > 65535)
            {
                _logger.Error("Server configuration failed, URL:Port is invalid");
                return false;
            }
            Uri uri = new Uri($"{url}:{port}");
            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add(uri.ToString());
            return true;
        }

        public void Stop()
        {
            if (!_isRunning) {
                _logger.Info("Server is not running.");
            }
            _isRunning = false;
            if (_httpListener.IsListening)
            {
                _httpListener.Stop();
            }
            _httpListener.Close();
            // Reinitialize httpListener becasue the current one is disposed
            InitListener();
        
        }

        private void InitListener() {
            _isRunning = false;
            _httpListener = new HttpListener();
        }
    }
}
