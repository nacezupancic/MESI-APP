﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Models.SaveableCanvasModels
{
    public class CanvasPosition : SaveableObject
    {
        public double? CanvasLeft { get; set; }

        public double? CanvasTop { get; set; }
    }
}
