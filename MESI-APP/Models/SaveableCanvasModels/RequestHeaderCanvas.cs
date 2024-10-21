using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESI_APP.Models.SaveableCanvasModels
{
    public class RequestHeaderCanvas : CanvasPosition
    {
        public ObservableCollection<HeaderDTO> HeaderList { get; set; } = new ObservableCollection<HeaderDTO>();        

    }
}
