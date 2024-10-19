
namespace MESI_APP.Models
{
    public class ReceivedRequestDTO
    {
        public DateTime Timestamp {  get; set; }
        public string Message { get; set; }
        public string Headers { get; set; }
        public string Method { get; set; }
        public ReceivedRequestDTO(DateTime timestamp, string message, string headers, string method) { 
            Timestamp = timestamp;
            Message = message;
            Headers = headers;
            Method = method;
        }
        public ReceivedRequestDTO() { }
    }
}
