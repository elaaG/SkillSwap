namespace SkillSwap.API.Exceptions
{
    public class SkillSwapException : Exception
    {
        public int StatusCode { get; set; }
        
        public SkillSwapException(string message, int statusCode = 400) 
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
    
    public class NotFoundException : SkillSwapException
    {
        public NotFoundException(string message) 
            : base(message, 404)
        {
        }
    }
    
    public class BadRequestException : SkillSwapException
    {
        public BadRequestException(string message) 
            : base(message, 400)
        {
        }
    }
    
    public class UnauthorizedException : SkillSwapException
    {
        public UnauthorizedException(string message = "Unauthorized access") 
            : base(message, 401)
        {
        }
    }
    
    public class InsufficientFundsException : SkillSwapException
    {
        public InsufficientFundsException(string message = "Insufficient credits in wallet") 
            : base(message, 400)
        {
        }
    }
}