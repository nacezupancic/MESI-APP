using MESI_APP.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Models.SaveableCanvasModels
{
    public class WindowSize : CanvasPosition
    {
        public int Width { get; set; } = Constants.WindowDefaultWidth;
        public int Height { get; set; } = Constants.WindowDefaultHeight;
    }
}
