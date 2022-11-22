using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;
using TechDemo.Core.Constants;

namespace TechDemo.Data.Entities
{
    
    [PgName("user")]
    public class User
    {
        [Key]
        [PgName("id")]
        public Guid Id { get; set; }
        
        [PgName("username")]
        public string Username { get; set; } = null!;
        
        
        [PgName("credentials")]
        [Column(TypeName = "jsonb")]
        public string? Credentials { get; set; }
        
        //TODO This could be extended into a full rbac table//allow multiple grants/policies/roles etc.
        
        [PgName("role")]
        public string Role { get; set; } = null!;
    }
}