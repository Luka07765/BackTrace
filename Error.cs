namespace Trace
{
    using HotChocolate;
    using HotChocolate.Execution;
    using Microsoft.IdentityModel.Tokens;

    public class GraphQLAuthErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            if (error.Exception is SecurityTokenExpiredException ||
                error.Exception is UnauthorizedAccessException)
            {
                return error
                    .WithMessage("Unauthorized")
                    .WithCode("UNAUTHENTICATED");
            }

            return error;
        }
    }

}
