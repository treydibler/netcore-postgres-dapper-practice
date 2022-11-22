using System.ComponentModel.DataAnnotations;

namespace TechDemo.DTO.Groups
{
    public class UpdateGroupRequest
    {
        [Required]
        public Guid Id { get; set; }
        
        [StringLength(255)]
        public string Name { get; set; } = null!;
    }   
}