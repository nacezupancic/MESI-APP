﻿using MESI_APP.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Models
{
    public class LoggerMsg
    {
        public LogErrorEnum Type { get; set; }
        public string Msg { get; set; }
        public DateTime LoggedAt { get; set; }
        public LoggerMsg(LogErrorEnum type, string msg ) {
            Type = type;
            Msg = msg;
            LoggedAt = DateTime.Now;
        }  
    }
}
