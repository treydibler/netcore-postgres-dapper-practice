using System.Security.Authentication;
using System.Security.Claims;
using TechDemo.Core;

namespace TechDemo.Web.Extensions
{
    public static class RequestExtensions
    {
        
        public static AuthenticatedApiRequest CreateAuthenticatedContext(this HttpContext request)
        {
            return new AuthenticatedApiRequest()
            {
                UserId = request.GetUserId()
            };
        }
        public static AuthenticatedApiRequest<T> CreateAuthenticatedContext<T>(this HttpContext request, T obj) where T : new()
        {
            return new AuthenticatedApiRequest<T>(obj)
            {
                UserId = request.GetUserId()
            };
        }

        public static Guid GetUserId(this HttpContext request)
        {
            return Guid.Parse(request.User.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new AuthenticationException("failed to process userId claim from jwt."));
        }
    }   
}