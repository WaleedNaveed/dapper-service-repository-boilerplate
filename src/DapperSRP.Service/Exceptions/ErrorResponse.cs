using System.Net;

namespace DapperSRP.Service.Exceptions
{
    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Title { get; set; }
        public string Exception { get; set; }
        public List<ErrorEntry> Entries { get; set; }
    }
}
