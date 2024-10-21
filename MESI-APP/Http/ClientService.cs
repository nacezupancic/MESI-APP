using MESI_APP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MESI_APP.Http
{
    public class ClientService
    {
        private HttpClient _httpClient;
        private readonly LoggerService _logger;

        public ClientService(LoggerService loggerService) {
            InitClient();
            _logger = loggerService;
        }

        private void InitClient() {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        public async Task<HttpResponseMessage> SendPostRequest(string url, string jsonString) {
            _logger.Info($"POST {url} was sent.");
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentException("URL cannot be null or empty");

                var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
                var resp = await _httpClient.PostAsync(url, httpContent);
                if (resp != null)
                {
                    _logger.Info($"Server responded with {resp.StatusCode} | {resp.Content}");
                }
                return resp;
            }
            catch (TaskCanceledException ex) {
                _logger.Error($"Request timedout: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error sending http request: {ex.Message}");
                return null;
            }

        }
    }
}
