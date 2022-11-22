using System.ComponentModel.DataAnnotations;

namespace TechDemo.DTO.Groups
{
    public class GroupTransfer
    {
        public Guid Id { get; set; }
        
        [StringLength(255, MinimumLength = 1)]
        [RegularExpression(@"\w+")]
        public string Name { get; set; } = null!;
    }   
}