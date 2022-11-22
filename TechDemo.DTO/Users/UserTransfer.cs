using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TechDemo.Core.Constants;

namespace TechDemo.DTO.Users
{
    public class UserTransfer 
    {
        
        [Required]
        public Guid Id { get; set; }
        
        [StringLength(255, MinimumLength = 3)]
        [EmailAddress]
        public string Username { get; set; } = null!;
        public string Role { get; set; } = AuthorizationRole.User;
    }
}