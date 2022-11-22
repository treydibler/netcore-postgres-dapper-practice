using System.ComponentModel.DataAnnotations;

namespace TechDemo.DTO.Groups
{
    public class AddGroupRequest
    {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$")]
        public string Name { get; set; } = null!;
        
        [Required]
        public List<Guid> Users { get; set; } = new List<Guid>();
    }
}