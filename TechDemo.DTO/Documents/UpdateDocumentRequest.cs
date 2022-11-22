using System.ComponentModel.DataAnnotations;

namespace TechDemo.DTO.Documents
{
    public class UpdateDocumentRequest : AddDocumentRequest
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
    }   
}