using MESI_APP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Services
{
    public interface ILoggerService
    {
        event Action<LoggerMsg> OnLog;
        void Info(string msg);
        void Error(string msg);
    }
}
