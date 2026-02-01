using Microsoft.AspNetCore.Http;
namespace Core.Exceptions
{
    public class CustomExceptions
    {
    }
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
    public sealed class BusinessException : AppException
    {
        public BusinessException(string message)
            : base(message, StatusCodes.Status400BadRequest)
        {
        }
    }

    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message)
            : base(message, StatusCodes.Status404NotFound)
        {
        }
    }

    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message)
            : base(message, StatusCodes.Status401Unauthorized)
        {
        }
    }

}
