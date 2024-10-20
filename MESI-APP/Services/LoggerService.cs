﻿using MESI_APP.Models;
using MESI_APP.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Services
{
    public class LoggerService
    {
        public event Action<LoggerMsg> OnLog;

        public void Log(string requestedfrom, LogErrorEnum type, string msg)
        {
            OnLog?.Invoke(new LoggerMsg(requestedfrom, type, msg));
        }
    }
}