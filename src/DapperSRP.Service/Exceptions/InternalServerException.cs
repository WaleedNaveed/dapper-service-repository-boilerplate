using System.Runtime.Serialization;

namespace DapperSRP.Service.Exceptions
{
    [Serializable]
    public class InternalServerException : Exception
    {
        public InternalServerException(string message) : base(message)
        {
        }

        public InternalServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InternalServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
