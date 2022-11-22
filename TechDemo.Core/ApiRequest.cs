namespace TechDemo.Core
{
    public class ApiRequest
    {
        
    }

    public class ApiRequest<T> : ApiRequest where T : new()
    {
        public T Data { get; set; } = default(T)!;
    }

    public class AuthenticatedApiRequest : ApiRequest
    {
        public Guid UserId { get; set; }
    }
    
    public class AuthenticatedApiRequest<T> : ApiRequest<T> where T : new()
    {
        public Guid UserId { get; set; }
        public AuthenticatedApiRequest(T obj)
        {
            Data = obj;
        }
    }
}