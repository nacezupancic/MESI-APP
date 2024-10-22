using MESI_APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Http
{
    public interface IServerService
    {
        Task Start();
        bool ConfigServer(string url, int port);
        void Stop();
        event Action<ReceivedRequestDTO> RequestReceived;
    }
}
