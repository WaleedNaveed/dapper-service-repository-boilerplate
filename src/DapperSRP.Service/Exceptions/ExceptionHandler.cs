using System.Net;

namespace DapperSRP.Service.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        public ExceptionHandler()
        {
        }

        public ServiceResponse<ErrorResponse> HandleException(System.Exception exception)
        {
            var error = exception switch
            {
                BadRequestException badRequestException => HandleBadRequestException(badRequestException),
                NotFoundException resourceNotFoundException => HandleResourceNotFoundException(resourceNotFoundException),
                UnAuthorizedException unauthorizedException => HandleUnauthorizedException(unauthorizedException),
                _ => HandleUnhandledExceptions(exception)
            };

            return error;
        }

        private ServiceResponse<ErrorResponse> HandleResourceNotFoundException(NotFoundException resourceNotFoundException)
        {
            return new ServiceResponse<ErrorResponse>()
            {

                ErrorMessage = resourceNotFoundException.Message,
                ErrorCode = (int)HttpStatusCode.NotFound,
                HasError = true,
                Result = null
            };
        }

        private ServiceResponse<ErrorResponse> HandleBadRequestException(BadRequestException badRequestException)
        {
            return new ServiceResponse<ErrorResponse>()
            {

                ErrorMessage = badRequestException.Message,
                ErrorCode = (int)HttpStatusCode.BadRequest,
                HasError = true,
                Result = null
            };
        }

        private ServiceResponse<ErrorResponse> HandleUnauthorizedException(UnAuthorizedException unauthorizedException)
        {
            return new ServiceResponse<ErrorResponse>()
            {

                ErrorMessage = unauthorizedException.Message,
                ErrorCode = (int)HttpStatusCode.Unauthorized,
                HasError = true,
                Result = null
            };
        }
        private ServiceResponse<ErrorResponse> HandleUnhandledExceptions(System.Exception exception)
        {
            return new ServiceResponse<ErrorResponse>()
            {

                ErrorMessage = exception.Message != "" ? exception.Message : "An unhandled error occurred while processing this request.",
                ErrorCode = (int)HttpStatusCode.InternalServerError,
                HasError = true,
                Result = null
            };
        }
    }
}
