namespace MESI_APP.Models.SaveableCanvasModels
{
    public class PortCanvas : CanvasPosition
    {
        private int? _port;
        public int? Port
        {
            get => _port; set
            {
                if (value > 0 && value < 65537)
                {
                    _port = value;
                }
            }
        }
    }
}
