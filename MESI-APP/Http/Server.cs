using System.Diagnostics;
using System.IO;
using System.Net;

namespace MESI_APP.Http
{
    public class Server 
    {
        private HttpListener _httpListener;
        public event Action<string> RequestReceived;
        private bool _isRunning;
        public Server() {
            InitListener();
        }
        public async Task Start()
        {
            try
            {
                if (_isRunning)
                    return;
                _isRunning = true;
                _httpListener.Start();
                await Task.Run(async () =>
                {
                    while (_isRunning)
                    {
                        var context = await _httpListener.GetContextAsync();
                        await HandleRequest(context);
                    }
                });

            }
            catch (Exception ex)
            {
                _isRunning = false;
                Debug.WriteLine($"{ex.Message}");
            }
        }

        private async Task HandleRequest(HttpListenerContext context)
        {
            var response = context.Response;
            var headers = context.Request.Headers;
            string content = "";
            using (var stream = new StreamReader(context.Request.InputStream))
            {
                content = stream.ReadToEnd();
            }

            RequestReceived?.Invoke(content);            
        }

        public void ConfigServer(string url, int port) {

            _httpListener.Prefixes.Clear();
            _httpListener.Prefixes.Add($"{url}:{port}/");
        }

        public void Stop()
        {
            _isRunning = false;
            _httpListener.Stop();
            _httpListener.Close();
            InitListener();
        }

        private void InitListener() {
            _isRunning = false;
            _httpListener = new HttpListener();
        }
    }
}
