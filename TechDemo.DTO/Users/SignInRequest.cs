using System.ComponentModel.DataAnnotations;

namespace TechDemo.DTO.Users
{
    public class SignInRequest
    {
        
        [StringLength(255, MinimumLength = 3)]
        [EmailAddress]
        public string Username { get; set; }
        
        
        //TODO can further regex to match password policy. 
        [MinLength(1)]
        public string Password { get; set; }
    }   
}