namespace DapperSRP.Service.Exceptions
{
    public interface IExceptionHandler
    {
        ServiceResponse<ErrorResponse> HandleException(System.Exception exception);
    }
}
