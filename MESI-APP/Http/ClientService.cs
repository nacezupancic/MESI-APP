using MESI_APP.Models.SaveableCanvasModels;
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
    public class ClientService : IClientService
    {
        private HttpClient _httpClient;
        private readonly ILoggerService _logger;

        public ClientService(ILoggerService loggerService) {
            InitClient();
            _logger = loggerService;
        }

        private void InitClient() {
            // Ignore ServerCertificate - Only for testing purposes, if server is running with invalid certificate
            _httpClient = new HttpClient(PrepareHandler());
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
        }

        private HttpClientHandler PrepareHandler() {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };
            return httpClientHandler;
        }

        public async Task<HttpResponseMessage> SendPostRequest(string url, IEnumerable<HeaderDTO> headers, string jsonString) {
            _logger.Info($"POST {url} was sent.");
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    throw new ArgumentException("URL cannot be null or empty");                           

                using (var newRequest = new HttpRequestMessage(HttpMethod.Post, url)) {
                    // Add headers
                    if (headers != null)
                    {
                        foreach (var h in headers)
                        {
                            newRequest.Headers.Clear(); // clear default headers
                            newRequest.Headers.Add(h.HeaderKey, h.HeaderValue);
                        }
                    }

                    using (var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json"))
                    {
                        newRequest.Content = httpContent;
                        var resp = await _httpClient.SendAsync(newRequest);
                        if (resp != null)
                        {
                            var content = await resp.Content.ReadAsStringAsync();
                            _logger.Info($"Server responded with {resp.StatusCode} | {content}");
                        }
                        return resp;
                    }
                }
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
