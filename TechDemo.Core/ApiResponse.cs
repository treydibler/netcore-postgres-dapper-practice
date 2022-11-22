using System.Net;

namespace TechDemo.Core
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }
    }
}