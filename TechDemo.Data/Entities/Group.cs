using System.ComponentModel.DataAnnotations;
using NpgsqlTypes;

namespace TechDemo.Data.Entities
{
    
    [PgName("group")]
    public class Group
    {
        [Key]
        [PgName("id")]
        public Guid Id { get; set; }
        
        
        [PgName("name")]
        [StringLength(255)]
        public string Name { get; set; } = null!;
    }   
}