using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TechDemo.DTO.Users
{
    public class AddUserRequest
    {
        
        [Required]
        [EmailAddress]
        [StringLength(320, MinimumLength = 3)]
        public string Username { get; set; } = null!;
        
        [Required]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = null!;
        
        [RegularExpression("USER|MANAGER|ADMINISTRATOR")]
        public string Role { get; set; } = null!;
    }
}