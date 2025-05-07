using System.Text.Json.Serialization;

namespace DapperSRP.Service
{
    public class ServiceResponse<T>
    {
        public T Result { get; set; }
        public bool HasError { get; set; }
        public int ErrorCode { get; set; }

        private string _errorMessage;

        [JsonIgnore]
        public Exception Exception { get; internal set; }

        public string ErrorMessage
        {
            get
            {
                if (Exception == null)
                    return _errorMessage;

                return Exception.Message;
            }
            set
            {
                _errorMessage = value;
            }
        }

        [JsonIgnore]
        public string StackTrace
        {
            get
            {
                if (Exception == null)
                    return string.Empty;

                return Exception.StackTrace;
            }
        }
    }
}
