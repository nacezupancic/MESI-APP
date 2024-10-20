using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Models.SaveableCanvasModels
{
    public class CanvasPosition
    {
        public double? CanvasLeft { get; set; }

        public double? CanvasTop { get; set; }
        public string BindableName { get; set; }
    }
}
