using TechDemo.Core.Constants;
using TechDemo.Data.Entities;

namespace TechDemo.Tests.Common
{
    public static class TestUsers
    {
        public static User GOOD_USER() => new User()
        {
            Id = Guid.Empty,
            Username = "user123",
            Credentials = "ANYTHING THAT ISN'T NULL",
            Role = AuthorizationRole.User
        };
        
        public static User GOOD_MANAGER() => new User()
        {
            Id = Guid.Empty,
            Username = "manager123",
            Credentials = "ANYTHING THAT ISN'T NULL",
            Role = AuthorizationRole.Manager
        };
        
        public static User GOOD_ADMIN() => new User()
        {
            Id = Guid.Empty,
            Username = "admin123",
            Credentials = "ANYTHING THAT ISN'T NULL",
            Role = AuthorizationRole.Administrator
        };

    }   
}