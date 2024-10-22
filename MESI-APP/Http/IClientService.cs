using MESI_APP.Models.SaveableCanvasModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Http
{
    public interface IClientService
    {
        Task<HttpResponseMessage> SendPostRequest(string url, IEnumerable<HeaderDTO> headers, string jsonString);
    }
}
