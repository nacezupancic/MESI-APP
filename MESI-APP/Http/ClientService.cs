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

        public ClientService() {
            InitClient();
        }

        private void InitClient() {
            _httpClient = new HttpClient();
        }

        public async Task<HttpResponseMessage> SendPostRequest(string url, string jsonString) {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be null or empty");

            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, httpContent);

        }
    }
}
