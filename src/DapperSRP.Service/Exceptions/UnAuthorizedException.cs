using System.Runtime.Serialization;

namespace DapperSRP.Service.Exceptions
{
    [Serializable]
    public class UnAuthorizedException : Exception
    {
        public UnAuthorizedException(string message) : base(message)
        {
        }

        public UnAuthorizedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnAuthorizedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
