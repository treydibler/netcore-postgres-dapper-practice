using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;

namespace TechDemo.Data.Entities
{
    [PgName("group_access")]
    public class DocumentAccess
    {
        [Key]
        [PgName("id")]
        public Guid Id { get; set; }
        
        [Required]
        [PgName("document_id")]
        public Guid DocumentId { get; set; }
        
        [PgName("user_id")]
        public Guid? UserId { get; set; }
        
        
        [PgName("group_id")]
        public Guid? GroupId { get; set; }
    }   
}