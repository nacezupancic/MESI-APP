using MESI_APP.Models;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace MESI_APP.Http
{
    public class ServerService 
    {
        private HttpListener _httpListener;
        public event Action<ReceivedRequestDTO> RequestReceived;
        private bool _isRunning;
        public ServerService() {
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
            DateTime dt = DateTime.Now;
            var response = context.Response;
            var request = context.Request;
            var headers = request.Headers;
            string content = "";
            using (var stream = new StreamReader(request.InputStream))
            {
                content = stream.ReadToEnd();
            }

            RequestReceived?.Invoke(new ReceivedRequestDTO(dt,content,string.Join('|', request.Headers.AllKeys), request.HttpMethod));
            response.Close();
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
