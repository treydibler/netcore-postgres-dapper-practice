using TechDemo.DTO.Users;
using TechDemo.Data.Entities;

namespace TechDemo.Data.Mappers
{
    public static class UserMapper
    {
        public static UserTransfer ToTransfer(this User user)
        {
            return new UserTransfer()
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };
        }
        
        public static List<UserTransfer> ToTransfer(this List<User> users)
        {
            return users.Select(ToTransfer).ToList();
        }
    }    
}